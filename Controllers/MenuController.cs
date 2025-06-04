
using app_infra.Data.Repository;
using codegen.Data;
using codegen.Data.Repositories;
using codegen.Extensions.Identity;
using codegen.Interface.Identity;
using codegen.ViewModels;
using codegeneratorlib.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codegen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        public IMenuManager _menuManager;
        private IRoleMenu _roleMenu;

        public MenuController(IRepositoryWrapper wrapper, IRoleMenu roleMenu)
        {
            _menuManager = wrapper.MenuManager_Repository;
            _roleMenu = roleMenu;
        }

        [HttpGet("GetMenu")]
        public IActionResult GetMenuForUser()
        {
            List<MenuVM> menu = HttpContext.Session.GetObject<List<MenuVM>>("Menu");
            return Ok(menu);
        }

        [HttpGet("AllMenu")]
        public async Task<IActionResult> GetAllMenu()
        {
            List<MenuVM> allMenuItems = await _menuManager.GetAllMenuItems();

            return Ok(allMenuItems);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> PostMenu(MenuVM model)
        {
            List<MenuVM> allMenuItems = await _menuManager.GetAllMenuItems();
            MenuVM foundMenu;

            foreach (MenuVM currMenuItem in allMenuItems)
            {
                if (model.ParentId == null && !allMenuItems.Select(s => s.Name).ToList().Contains(model.Name)) foundMenu = new MenuVM();
                else foundMenu = await _menuManager.FindMenu(currMenuItem, model.ParentId);

                if (foundMenu != null && 
                    (!foundMenu.Submenus.Select(s => s.Name).ToList().Contains(model.Name)))
                {
                    // Remove link of parent menu then update
                    if(model.ParentId != null)
                    {
                        MenuVM parentMenu = await _menuManager.FindMenu(currMenuItem, currMenuItem.MenuId);
                        Menu saveparentMenu = new Menu()
                        {

                        };
                        saveparentMenu.Link = null;
                        saveparentMenu.UpdateDate = DateTime.Now;
                        saveparentMenu.UpdatedById = HttpContext.Session.GetObject<int>("UserId");
                        _menuManager.Update(saveparentMenu);
                    }

                    // Create the submenu
                    Menu newMenu = new Menu()
                    {
                        MenuName = model.Name,
                        MenuDescription = model.Description,
                        MenuSequence = model.MenuSequence,
                        Link = String.IsNullOrEmpty(model.Link) ? null : model.Link,
                        ParentId = model.ParentId,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        CreatedById = HttpContext.Session.GetObject<int>("UserId"),
                        UpdatedById = HttpContext.Session.GetObject<int>("UserId")
                    };

                    _menuManager.Add(newMenu);
                    _menuManager.SaveChangesAsync();

                    return Ok();
                }
            }
            return BadRequest("Error in adding menu item");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> PutMenu(MenuVM model)
        {
            List<MenuVM> allMenuItems = await _menuManager.GetAllMenuItems();
            MenuVM foundMenu;
            foreach (MenuVM rootItem in allMenuItems)
            {
                if (model.ParentId == null 
                    && (!allMenuItems.Where(s => s.MenuId != model.MenuId).Select(s => s.Name).ToList().Contains(model.Name))) foundMenu = new MenuVM();
                else
                    foundMenu = await _menuManager.FindMenu(rootItem, model.ParentId);

                if (foundMenu != null 
                    && (!foundMenu.Submenus.Where(s => s.MenuId != model.MenuId).Select(s => s.Name).Contains(model.Name) ))
                {
                    Menu updatedMenu = _menuManager.FindByCondition(m => m.MenuId == model.MenuId).FirstOrDefault();

                    updatedMenu.MenuName = model.Name;
                    updatedMenu.MenuDescription = model.Description;
                    updatedMenu.MenuSequence = model.MenuSequence;
                    updatedMenu.Link = String.IsNullOrEmpty(model.Link) ? null : model.Link;
                    updatedMenu.ParentId = model.ParentId;
                    updatedMenu.UpdateDate = DateTime.Now;
                    updatedMenu.UpdatedById = HttpContext.Session.GetObject<int>("UserId");
                    updatedMenu.ProcessControl = model.ProcessControl;

                    _menuManager.Update(updatedMenu);
                    await _menuManager.SaveChangesAsync();

                    return Ok();
                }
            }
            return BadRequest("Error in updating menu item");
        }

        [HttpDelete("delete/{menuId}")]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {
            List<MenuVM> wholeMenu = await _menuManager.GetAllMenuItems();
            MenuVM foundMenu;
            foreach (MenuVM currMenuItem in wholeMenu)
            {
                foundMenu = await _menuManager.FindMenu(currMenuItem, menuId);
                if (foundMenu != null)
                {
                    await _menuManager.DeleteMenu(foundMenu);
                    int currUserId = HttpContext.Session.GetObject<int>("UserId");
                    HttpContext.Session.SetObject("Menu", await _menuManager.GenerateMenuForCurrUser(currUserId));
                    return Ok();
                }
            }
            return BadRequest("Error in deleting a menu item");
        }
       
        [HttpPost("EditRoleMenuAssignment")]
        public async Task<IActionResult> EditRoleMenu(RoleMenuEditVM model)
        {
            Menu menu = _menuManager.FindFirst(m => m.MenuId == model.MenuId);
            List<RoleMenu> roleMenus = _roleMenu.FindByCondition(r => r.MenuId == model.MenuId).ToList();

            if (menu == null) return NotFound("Record not found");

            foreach(int assignmentId in model.RoleAssignments)
            {
                if (!roleMenus.Select(r => r.RoleId).ToList().Contains(assignmentId))
                {
                    RoleMenu newRoleMenu = new RoleMenu()
                    {
                        MenuId = model.MenuId,
                        RoleId = assignmentId,
                        UpdateDate = DateTime.Now,
                        UpdatedById = HttpContext.Session.GetObject<int>("UserId")
                    };
                    _roleMenu.Add(newRoleMenu);
                }
            }

            foreach (RoleMenu roleMenu in roleMenus)
            {
                if (!model.RoleAssignments.Contains(roleMenu.RoleId))
                {
                    _roleMenu.Delete(roleMenu);
                }
            }

            try
            {
                await _roleMenu.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Cannot update assignment of menu to a role");
            }
        }
    }
}