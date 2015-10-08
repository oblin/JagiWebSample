using JagiWebSample.Areas.Admin.Models;
using JagiWebSample.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set { _roleManager = value; }
        }

        // GET: Admin/User
        public ActionResult Index(string search)
        {
            IEnumerable<ApplicationUser> users;
            if (string.IsNullOrEmpty(search))
                users = UserManager.Users;
            else
                users = UserManager.Users.Where(p => p.UserName.Contains(search) || p.Email.Contains(search));

            bool isRolesEnabled = true;
            string[] roles = null;
            if (RoleManager != null && RoleManager.Roles.Any())
            {
                roles = RoleManager.Roles.Select(p => p.Name).ToArray();
            }
            else
            {
                isRolesEnabled = false;
            }
            ViewBag.OnlineUsers = GetOnelineUsers(users);

            return View(new UserManageIndex {
                Search = search,
                Users = users,
                Roles = roles,
                IsRolesEnabled = isRolesEnabled,
                OnlineUsers = GetOnelineUsers(users)
            });
        }

        private IEnumerable<ApplicationUser> GetOnelineUsers(IEnumerable<ApplicationUser> users)
        {
            // 計算 30 分鐘內登入的都算是 Online
            DateTime DateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30.0));
            return users.Where(u => u.LastActivityDate > DateActive);
        }
    }
}