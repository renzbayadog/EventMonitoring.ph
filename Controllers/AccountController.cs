using codegen.Data;
using codegen.Extensions.Identity;
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
    public class AccountController : ControllerBase
    {
        private db_ab9d6a_dbrenz _context;
        private UserManager<User> _userManager;
        private IEmailManager _emailManager;

        public AccountController(db_ab9d6a_dbrenz context, UserManager<User> usrMgr, IEmailManager emailMgr)
        {
            _context = context;
            _userManager = usrMgr;
            _emailManager = emailMgr;
        }
       
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ProfileVM Profile)
        {
            // Check if email or username is taken
            User user = await _userManager.Users.Where(u => u.Id == Profile.UserDetails.Id).FirstOrDefaultAsync();
            if (user == null) return NotFound("User not found");

            bool isEmailTaken = (await _userManager.Users.Where(u => u.Id != Profile.UserDetails.Id
                                                                && u.Email == Profile.UserDetails.Email).FirstOrDefaultAsync()) != null ? true : false;
            if (isEmailTaken) return BadRequest("Email is already taken");

            // Modify User Table
            user.FirstName = Profile.UserDetails.FirstName;
            user.MiddleInitial = Profile.UserDetails.MiddleInitial;
            user.LastName = Profile.UserDetails.LastName;
            user.Email = Profile.UserDetails.Email;
            user.PhoneNumber = Profile.UserDetails.PhoneNumber;

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

            // Modify Agent Details
            //Agent agent = await _context.Agents.Where(a => a.AgentId == Profile.UserDetails.Id).FirstOrDefaultAsync();
            //if (agent != null)
            //{
            //    agent.AgentInitials = Profile.AgentDetails.AgentInitials;
            //    agent.Address = Profile.AgentDetails.Address;
            //    agent.Sex = Profile.AgentDetails.Sex;

            //    _context.Update(agent);
            //    _context.SaveChanges();
            //}

            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(UserChangePassword model)
        {
            if (!ModelState.IsValid) return BadRequest("Validation error");

            User user = _userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null) return NotFound("User not found");

            PasswordVerificationResult verifiedPassword = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
            if(!verifiedPassword.Equals(PasswordVerificationResult.Success )) return BadRequest("Old password incorrect");

            if (string.IsNullOrEmpty(model.NewPassword)) return BadRequest("Enter your new password");
            
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            UserVM userVM = new UserVM
            {
                FirstName = user.FirstName,
                MiddleInitial = user.MiddleInitial,
                LastName = user.LastName,
                Email = user.Email
            };
            //_emailManager.EmailChangePassword(userVM);

            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(UserResetPassword model)
        {
            if (!ModelState.IsValid) return BadRequest("Validation error");

            User user = _userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null) return NotFound("User not found");

            //PasswordVerificationResult verifiedPassword = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
            //if (!verifiedPassword.Equals(PasswordVerificationResult.Success)) return BadRequest("Old password incorrect");

            if (string.IsNullOrEmpty(model.NewPassword)) return BadRequest("Enter your new password");

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            UserVM userVM = new UserVM
            {
                FirstName = user.FirstName,
                MiddleInitial = user.MiddleInitial,
                LastName = user.LastName,
                Email = user.Email
            };

            //_emailManager.EmailChangePassword(userVM);

            return Ok();
        }
    }
}
