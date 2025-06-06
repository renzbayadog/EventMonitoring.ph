using codegen.Data;
using codegen.Extensions.Identity;
using codegen.ViewModels;
using codegeneratorlib.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codegen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private AppRoleContext _context;
        private RoleManager<Role> _roleManager;

        public RoleController(AppRoleContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet("list")]
        public IActionResult GetRole()
        {
            List<Role> roles = _context.Roles.ToList();
            List<RoleVM> rolesVM = new List<RoleVM>();

            foreach (Role role in roles)
            {
                rolesVM.Add(new RoleVM()
                {
                    RoleId = role.Id,
                    Name = role.Name,
                    Description = role.RoleDescription ?? ""
                });
            }

            return Ok(rolesVM);
        }

        [HttpPost("add")]
        public async Task<IActionResult> PostRole(RoleVM model)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if(roleExists) return BadRequest("Role already exists!");

            Role role = new Role() {
                Name = model.Name,
                RoleDescription = model.Description,
                CreateDate = DateTime.Today,
                UpdateDate = DateTime.Today,
                CreatedById = HttpContext.Session.GetObject<int>("UserId"),
                UpdatedById = HttpContext.Session.GetObject<int>("UserId")
            };

            await _roleManager.CreateAsync(role);

            return Ok("Role Successfully Created");
        }

        [HttpPost("{roleId}/update")]
        public async Task<IActionResult> PutRole(RoleVM model, int roleId)
        {
            Role role = await _roleManager.Roles.Where(r => r.Id == roleId).SingleOrDefaultAsync();

            bool roleExists = (await _roleManager.Roles.Where(r => r.Id != model.RoleId && r.Name == model.Name).FirstOrDefaultAsync()) != null ? true : false;
            if (roleExists) return BadRequest("Role already exists");

            role.Name = model.Name;
            role.RoleDescription = model.Description;
            role.UpdatedById = HttpContext.Session.GetObject<int>("UserId");
            role.UpdateDate = DateTime.Today;

            IdentityResult result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRoleAsync(int roleId)
        {
            Role role = await _roleManager.Roles.Where(r => r.Id == roleId).SingleOrDefaultAsync();

            if (role == null) return NotFound("Role does not exist");

            List<IdentityUserRole<int>> test = _context.UserRoles.Where(ur => ur.RoleId == role.Id).ToList();

            if (_context.UserRoles.Where(ur => ur.RoleId == role.Id).Any()) return BadRequest("Unable to delete role");

            IdentityResult result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded) return BadRequest("Error in deleting role");

            return Ok();
        }
    }
}