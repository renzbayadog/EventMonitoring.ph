using codegen.Data;
using codegen.Interface.Identity;
using codegen.ViewModels;
using codegeneratorlib.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EventMonitoring.ph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAccount _account;
        private IMenuManager _menuManager;

        public AuthController(IAccount account, IMenuManager menuManager)
        {
            _account = account;
            _menuManager = menuManager;
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(UserLoginRequestDTO LoginModel)
        {
            var response = await _account.LoginAsync(LoginModel);

            var userClaim = response.ClaimGetUserResponse;

            if(userClaim != null)
            {

                // var result = await ProtectedSessionStore.GetAsync<int>("count");
                // var currentCount = result.Success ? result.Value : 0;

                //HttpContext.Session.SetObject("UserFullName", $"{userClaim.UserInfo.FirstName} {userClaim.UserInfo.LastName}");
                //HttpContext.Session.SetObject("UserId", userClaim.UserInfo.Id);
                //HttpContext.Session.SetObject("RoleId", userClaim.UserInfo.RoleId);
                //HttpContext.Session.SetObject("UserRole", userClaim.UserInfo.RoleName);
                //HttpContext.Session.SetObject("UserRoleDescription", userClaim.UserInfo.RoleDescription);
                //HttpContext.Session.SetObject("UserName", userClaim.UserInfo.UserName);

                //var menulist = _menuManager.GenerateMenuForCurrUser(userClaim.UserInfo.Id).Result;

                //if (menulist != null)
                //{
                //    HttpContext.Session.SetObject("Menu", menulist);
                //}

                //switch (role.Name)
                //{
                //    case "CC":
                //        HttpContext.Session.SetObject("Summary", "Sales Summary");
                //        break;
                //    case "RBM":
                //        HttpContext.Session.SetObject("Summary", "Sales Summary");
                //        break;
                //    default:
                //        HttpContext.Session.SetObject("Summary", "National");
                //        break;
                //}
            }

            return Ok(response);
        }
    }
}