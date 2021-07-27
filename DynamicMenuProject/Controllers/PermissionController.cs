using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicMenuProject.Data;
using DynamicMenuProject.Helpers;
using DynamicMenuProject.Models;
using DynamicMenuProject.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DynamicMenuProject.Controllers
{
    [AuthorizeActionFilter]
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PermissionController(ApplicationDbContext context)
        {
            this._context = context;
        }
        
        public IActionResult GetMenus()
        {
            List<MenuViewModel> lstmodel = new List<MenuViewModel>();
            var models = _context.MenuItems;//s.Where(m=>m.MenuGrpId!=0).OrderBy(m=>m.MenuGrpId);

            foreach (var item in models)
            {
                MenuViewModel mvm = new MenuViewModel();
                mvm.Id = item.Id;
                mvm.Name = item.Name;
                mvm.Path = item.Path;
                mvm.ParentId = item.ParentId;
                mvm.MenuLevel = item.MenuLevel;
                mvm.MenuGrpId = item.MenuGrpId;

                lstmodel.Add(mvm);
            }

            ViewBag.successMessage = TempData["successMessage"];
            return View(lstmodel);
        }
        
        [HttpGet]
        public IActionResult CreateMenu()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateMenu(MenuViewModel model)
        {
            if (model != null)
            {
                var menuModel = new MenuItems();
                menuModel.Name = model.Name;
                menuModel.Path = model.Path;
                menuModel.ParentId = model.ParentId;
                menuModel.MenuLevel = model.MenuLevel;
                menuModel.MenuGrpId = model.MenuGrpId;

                _context.MenuItems.Add(menuModel);
                _context.SaveChanges();

                return RedirectToAction("GetMenus");
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditMenu(int Id)
        {
            MenuViewModel mvm = new MenuViewModel();
            var menu = _context.MenuItems.FirstOrDefault(m => m.Id == Id);

            if (menu != null)
            {
                mvm.Id = menu.Id;
                mvm.Name = menu.Name;
                mvm.Path = menu.Path;
                mvm.ParentId = menu.ParentId;
                mvm.MenuLevel = menu.MenuLevel;
                mvm.MenuGrpId = menu.MenuGrpId;
            }
            return View(mvm);
        }

        [HttpGet]
        public IActionResult DeleteMenu(int Id)
        {
            var permission = _context.MenuPermissions.Where(m => m.MenuId == Id);
            foreach (var perm in permission)
            {
                _context.MenuPermissions.Remove(perm);
            }
            var menu = _context.MenuItems.FirstOrDefault(m => m.Id == Id);
            _context.MenuItems.Remove(menu);
            _context.SaveChanges();

            TempData["successMessage"] = "Menu Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("GetMenus");

        }

        [HttpPost]
        public IActionResult EditMenu(MenuViewModel model)
        {
            if (model != null)
            {
                var MenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == model.Id);
                if (MenuItem != null)
                {
                    MenuItem.Name = model.Name;
                    MenuItem.Path = model.Path;
                    MenuItem.ParentId = model.ParentId;
                    MenuItem.MenuLevel = model.MenuLevel;
                    MenuItem.MenuGrpId = model.MenuGrpId;

                    _context.Entry(MenuItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    return RedirectToAction("GetMenus");
                }
                return View();
            }
            return View();
        }
        
        public IActionResult Index()
        {
            var roleNames = (from roles in _context.Roles
                             join
                             menup in _context.MenuPermissions
                             on roles.Id equals Convert.ToString(menup.RoleId)
                             select new PermissionRoleViewModel
                             {
                                 Name = roles.Name,
                                 RoleId = roles.Id
                             }).GroupBy(n => new { n.RoleId, n.Name }).Select(g => g.FirstOrDefault()).ToList();

            return View(roleNames);
        }

        [HttpGet]
        public IActionResult CreatePermissions(Guid RoleId)
        {
            List<PermissionViewModel> lstPermission = new List<PermissionViewModel>();

            var result = (from Menus in _context.MenuItems
                          join Permissions in _context.MenuPermissions.Where(r => r.RoleId == RoleId)
                          on Menus.Id equals Permissions.MenuId into menuPerm
                          from perm in menuPerm.DefaultIfEmpty()
                          select new PermissionViewModel
                          {
                              Id = Menus.Id,
                              Path = Menus.Path,
                              Name = Menus.Name,
                              ParentId = Menus.ParentId,
                              MenuLevel = Menus.MenuLevel,
                              HasAccess = perm == null ? false : !(string.IsNullOrEmpty(Convert.ToString(perm.RoleId)))
                          }).ToList();

            var RolesList = (from roles in _context.Roles
                             select new SelectListItem
                             {
                                 Value = roles.Id,
                                 Text = roles.Name
                             }).ToList();
            ViewBag.RolesList = RolesList;
            return View(result);
        }


        [HttpPost]
        public JsonResult CreatePermissions([FromBody]UpdatePermissionViewModel[] model)
        {
            if (model != null)
            {
                var existing = _context.MenuPermissions.Where(R => R.RoleId == model.FirstOrDefault().RoleId).ToList();
                if (existing != null && existing.Count() > 0)
                {
                    return Json("Permission already assigned!");

                }
                else
                {
                    foreach (var item in model)
                    {
                        MenuPermissions mnp = new MenuPermissions();
                        mnp.MenuId = item.MenuId;
                        mnp.PermissionId = Guid.NewGuid();
                        mnp.RoleId = item.RoleId;
                        _context.MenuPermissions.Add(mnp);

                        _context.SaveChanges();
                    }
                    return Json("Saved Successfully!");
                }
            }
            return Json("NoData");
        }

        [HttpGet]
        public IActionResult ViewPermissions(Guid RoleId)
        {
            List<PermissionViewModel> lstPermission = new List<PermissionViewModel>();

            var result = (from Menus in _context.MenuItems
                          join Permissions in _context.MenuPermissions.Where(r => r.RoleId == RoleId)
                          on Menus.Id equals Permissions.MenuId into menuPerm
                          from perm in menuPerm.DefaultIfEmpty()
                          select new PermissionViewModel
                          {
                              Id = Menus.Id,
                              Path = Menus.Path,
                              Name = Menus.Name,
                              ParentId = Menus.ParentId,
                              MenuLevel = Menus.MenuLevel,
                              HasAccess = perm == null ? false : !(string.IsNullOrEmpty(Convert.ToString(perm.RoleId)))
                          }).ToList();

            var RolesList = (from roles in _context.Roles
                             select new SelectListItem
                             {
                                 Value = roles.Id,
                                 Text = roles.Name
                             }).ToList();
            ViewBag.RolesList = RolesList;
            return View(result);
        }

        [HttpGet]
        public IActionResult DeletePermissions(Guid RoleId)
        {
            var permission = _context.MenuPermissions.Where(m => m.RoleId == RoleId);
            foreach (var perm in permission)
            {
                _context.MenuPermissions.Remove(perm);
            }
            var perms = _context.MenuPermissions.FirstOrDefault(m => m.RoleId == RoleId);
            _context.MenuPermissions.Remove(perms);
            _context.SaveChanges();

            TempData["successMessage"] = "Permissions Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult EditPermissions(Guid RoleId)
        {
            ViewBag.RoleId = RoleId;
            //List<PermissionViewModel> lstPermission = new List<PermissionViewModel>();

            var result = (from Menus in _context.MenuItems
                          join Permissions in _context.MenuPermissions.Where(r => r.RoleId == RoleId)
                          on Menus.Id equals Permissions.MenuId into menuPerm
                          from perm in menuPerm.DefaultIfEmpty()
                          select new PermissionViewModel
                          {
                              Id = Menus.Id,
                              Path = Menus.Path,
                              Name = Menus.Name,
                              ParentId = Menus.ParentId,
                              MenuLevel = Menus.MenuLevel,
                              HasAccess = perm == null ? false : !(string.IsNullOrEmpty(Convert.ToString(perm.RoleId)))
                          }).ToList();

            //var RolesList = (from roles in _context.Roles
            //                 select new SelectListItem
            //                 {
            //                     Value = roles.Id,
            //                     Text = roles.Name
            //                 }).ToList();
            //ViewBag.RolesList = RolesList;
            return View(result);
        }

        [HttpPost]
        public JsonResult EditPermissions([FromBody]UpdatePermissionViewModel[] model)
        {
            if (model != null)
            {

                int count = 0;
                foreach (var item in model)
                {
                    if (count == 0)
                    {
                        var existing = _context.MenuPermissions.Where(R => R.RoleId == item.RoleId);

                        foreach (var roles in existing)
                        {
                            _context.MenuPermissions.Remove(roles);
                        }
                        count++;
                    }
                    MenuPermissions mnp = new MenuPermissions();
                    mnp.MenuId = item.MenuId;
                    mnp.PermissionId = Guid.NewGuid();
                    mnp.RoleId = item.RoleId;
                    _context.MenuPermissions.Add(mnp);
                }

                _context.SaveChanges();

                return Json("Saved Successfully!");
            }
            return Json("NoData");
        }

        [HttpGet]
        public IActionResult GetPermissions(Guid RoleId)
        {
            List<PermissionViewModel> lstPermission = new List<PermissionViewModel>();

            var result = (from Menus in _context.MenuItems
                          join Permissions in _context.MenuPermissions.Where(r => r.RoleId == RoleId)
                          on Menus.Id equals Permissions.MenuId into menuPerm
                          from perm in menuPerm.DefaultIfEmpty()
                          select new PermissionViewModel
                          {
                              Id = Menus.Id,
                              Path = Menus.Path,
                              Name = Menus.Name,
                              ParentId = Menus.ParentId,
                              MenuLevel = Menus.MenuLevel,
                              HasAccess = perm == null ? false : !(string.IsNullOrEmpty(Convert.ToString(perm.RoleId)))
                          }).ToList();

            var RolesList = (from roles in _context.Roles
                             select new SelectListItem
                             {
                                 Value = roles.Id,
                                 Text = roles.Name
                             }).ToList();
            ViewBag.RolesList = RolesList;
            return View(result);
        }

        [HttpPost]
        public JsonResult UpdatePermission([FromBody]UpdatePermissionViewModel[] model)
        {
            if (model != null)
            {

                int count = 0;
                foreach (var item in model)
                {
                    if (count == 0)
                    {
                        var existing = _context.MenuPermissions.Where(R => R.RoleId == item.RoleId);

                        foreach (var roles in existing)
                        {
                            _context.MenuPermissions.Remove(roles);
                        }
                        count++;
                    }
                    MenuPermissions mnp = new MenuPermissions();
                    mnp.MenuId = item.MenuId;
                    mnp.PermissionId = Guid.NewGuid();
                    mnp.RoleId = item.RoleId;
                    _context.MenuPermissions.Add(mnp);
                }

                _context.SaveChanges();

                return Json("Saved Successfully!");
            }
            return Json("NoData");
        }
    }
}