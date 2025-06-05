using codegen.Data;
using codegen.Extensions.Identity;
using codegeneratorlib.Helpers;
using codegen.Interface.Identity;
using codegen.ViewModels;
using codegeneratorlib.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codegen.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IFtpManager _ftpManager;
        private readonly IEmailManager _emailManager;
        private readonly IAccount _accountService;

        private readonly IUserManager _userManager1;

        public UserController(UserManager<User> userManager,
                              IUserManager userManager1,
                              IFtpManager ftpManager,
                              IEmailManager emailManager,
                              IAccount accountService)
        {
            _userManager = userManager;
            _userManager1 = userManager1;
            _accountService = accountService;
            _ftpManager = ftpManager;
            _emailManager = emailManager;
        }

        [HttpGet]
        [Route("List/Page{currPage:int}")]
        [Route("List")]
        public async Task<IActionResult> GetUserAsync([FromQuery] UserSearch searchInfo, int currPage = 1, int pageSize = 10)
        {
            var CDNFTPConfiguration = AppHelper.CDNFTPConfiguration;
            CDNFTPConfiguration.FTPFolderVM = AppHelper.CDNFTPFolder;

            var oFileUrl = $@"{CDNFTPConfiguration.DriveDirectory}\{CDNFTPConfiguration.FTPFolderVM.ProfilePicLocation}";
            var publicURL = $"{CDNFTPConfiguration.PublicHost}/{CDNFTPConfiguration.FTPFolderVM.ProfilePicLocation}";

            List<UserVM> usersVM = await _userManager1.GetAllUsers(searchInfo);

            Pagination<UserVM> pagination = new Pagination<UserVM>(usersVM, currPage, pageSize);

            return Ok(pagination);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> PostUser(UserVM model)
        {
            if (!ModelState.IsValid) return BadRequest("Recheck input values");

            bool isEmailTaken = (await _userManager.FindByEmailAsync(model.Email)) != null ? true : false;
            if (isEmailTaken) return BadRequest("Email is already taken.");

            bool isUsernameTaken = (await _userManager.FindByNameAsync(model.UserName)) != null ? true : false;
            if (isUsernameTaken) return BadRequest("Username is already taken.");
            // Insert in User Table
            User user = new User()
            {
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                CreatedById = HttpContext.Session.GetObject<int>("UserId"),
                UpdatedById = HttpContext.Session.GetObject<int>("UserId"),
                CreateDate = DateTime.Today,
                UpdateDate = DateTime.Today
            };

            model.Password = Guid.NewGuid().ToString("d").Substring(1, 8);

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                List<string> listError = new List<string>();
                foreach (IdentityError item in result.Errors)
                {
                    listError.Add(item.Description);
                }
                string listErr = string.Join(",", listError.ToArray());

                return BadRequest(listErr);
            }

            // Insert in UserRole Table
            UserRole userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = model.RoleId
            };

            await _userManager1.AddUserRoles(userRole);

            // Insert in Agent Table
            //Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
            //Agent agent = new Agent
            //{
            //    AgentId = user.Id,
            //    AgentInitials = $"{char.ToUpper(user.FirstName.First())}{(user.MiddleInitial == null ? String.Empty : user.MiddleInitial.First().ToString().ToUpper())}{char.ToUpper(user.LastName.First())}"
            //};

            //switch (role.Name.ToLower())
            //{
            //    case "cc":
            //        agent.RegionId = null;
            //        agent.RBMId = model.RBMId;
            //        agent.AgentInitials = model.UserName;
            //        _context.Agents.Add(agent);
            //        break;
            //    case "rbm":
            //        agent.RBMId = null;
            //        agent.AgentInitials = model.UserName;
            //        agent.RegionId = model.RegionId;
            //        _context.Agents.Add(agent);
            //        break;
            //}
            try
            {
                await _userManager1.SaveChangesAsync();
                //send temporary password
                //_emailManager.EmailTemporaryPassword(model);
                //_emailManager.SaveTemporaryPassword(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
            return Ok();
        }

        [HttpPost("Update")]
        public async Task<IActionResult> PutUser(UserVM model)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null) return NotFound("User not found");

            bool isEmailTaken = (await _userManager.Users.Where(u => u.Id != model.Id
                                                    && u.Email == model.Email).FirstOrDefaultAsync()) != null ? true : false;
            if (isEmailTaken) return BadRequest("Email is already taken");

            bool isUsernameTaken = (await _userManager.Users.Where(u => u.Id != model.Id
                                        && u.UserName == model.UserName).FirstOrDefaultAsync()) != null ? true : false;
            if (isUsernameTaken) return BadRequest("Username is already taken");

            // Update User Table
            user.FirstName = model.FirstName;
            user.MiddleInitial = model.MiddleInitial;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                List<string> listError = new List<string>();
                foreach (IdentityError item in result.Errors)
                {
                    listError.Add(item.Description);
                }
                return BadRequest(string.Join(",", listError.ToArray()));
            }

            // Update UserRole Table

            IdentityUserRole<int> userRole = await _userManager1.GetUserRole(model.Id);
            if (userRole != null)
            {
                _userManager1.RemoveUserRoles(userRole);
                await _userManager1.SaveChangesAsync();
            }
            UserRole newUserRole = new UserRole()
            {
                UserId = model.Id,
                RoleId = model.RoleId
            };

            await _userManager1.AddUserRoles(newUserRole);
            await _userManager1.SaveChangesAsync();

            // Update Agent Table
            //Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);

            //var roleName = role.Name.ToLower();

            //Agent existentAgent = await _context.Agents.FirstOrDefaultAsync(r => r.AgentId == model.Id);
            //Agent agent = new Agent { AgentId = user.Id };
            //switch (roleName)
            //{
            //    case "cc":
            //        if (existentAgent == null)
            //        {
            //            agent.RegionId = null;
            //            agent.RBMId = model.RBMId;
            //            agent.AgentInitials = model.UserName;
            //            _context.Agents.Add(agent);
            //        }
            //        else
            //        {
            //            existentAgent.RegionId = null;
            //            existentAgent.RBMId = model.RBMId;
            //            existentAgent.AgentInitials = model.UserName;
            //            _context.Agents.Update(existentAgent);
            //        }
            //        break;
            //    case "rbm":
            //        if (existentAgent == null)
            //        {
            //            agent.RegionId = model.RegionId;
            //            agent.RBMId = null;
            //            agent.AgentInitials = model.UserName;
            //            _context.Agents.Add(agent);
            //        }
            //        else
            //        {
            //            existentAgent.RegionId = model.RegionId;
            //            existentAgent.RBMId = null;
            //            existentAgent.AgentInitials = model.UserName;
            //            _context.Agents.Update(existentAgent);
            //        }
            //        break;
            //    default:
            //        if (existentAgent != null) _context.Agents.Remove(existentAgent);
            //        break;
            //}
            //_context.SaveChanges();
            return Ok();
        }

        [HttpPost("disable/{userId}")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            user.IsDeleted = true;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Error in disabling user account");

            return Ok();
        }

        [HttpPost("enable/{userId}")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            user.IsDeleted = false;

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Error in enabling user account");

            return Ok();
        }

        [HttpPost("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return BadRequest("User not found");

            // Delete in UserRole
            IdentityUserRole<int> userRole = await _userManager1.GetUserRole(userId);
            if (userRole != null)
            {
                _userManager1.DeleteUserRole(userRole.UserId, userRole.RoleId);
                await _userManager1.SaveChangesAsync();
            }
            await _userManager.DeleteAsync(user);

            return Ok();
        }
    }
}