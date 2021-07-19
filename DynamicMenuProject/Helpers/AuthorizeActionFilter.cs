using DynamicMenuProject.Data;
using DynamicMenuProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicMenuProject.Helpers
{
    public class AuthorizeActionFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userName = context.HttpContext.User.Identity.Name;
            var keys = context.RouteData.Values.Values;
            var path = "";
            int count = 0;
            foreach (var item in keys)
            {
                count++;
                if (count == 3)
                {
                    break;
                }
                path = path + "/" + item;
                //path = path + item;

            }

            var _dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);

            var result = (from menus in _dbContext.MenuPermissions
                          join menuItems in _dbContext.MenuItems.Where(m => m.Path != "" && m.Path != null)
                          on menus.MenuId equals menuItems.Id
                          join userRoles in _dbContext.UserRoles.Where(u => u.UserId == user.Id)
                          on menus.RoleId equals Guid.Parse(userRoles.RoleId)
                          select new
                          {
                              Path = menuItems.Path,
                          }).ToList();


            foreach (var item in result)
            {
                var resultpath = item.Path.Split("/");
                if (path == item.Path)
                {
                    return;
                }
                //else if (path=="/Permission/UpdatePermission")
                //{
                //    return;
                //}
                else if (resultpath[1] == path.Split("/")[1])
                {
                    return;
                }

            }

            context.Result = new RedirectResult("~/Home/AccessDenied");
            //context.Result = new UnauthorizedResult();
            return;
        }

    }
}
