using JagiWebSample.Areas.Admin.Models;
using JagiWebSample.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (RoleManager.Roles.Any())
            {
                roles = RoleManager.Roles.Select(p => p.Name).ToArray();
            }

            ViewBag.OnlineUsers = GetOnelineUsers(users);

            return View(new UserIndexView {
                Search = search,
                Users = users,
                Roles = roles,
                IsRolesEnabled = isRolesEnabled,
                OnlineUsers = GetOnelineUsers(users)
            });
        }

        public ActionResult UnApproval(int? page)
        {
            IEnumerable<ApplicationUser> users = UserManager.Users.Where(p => p.IsApproved == false);
            var roles = RoleManager.Roles.Select(p => p.Name).ToArray();

            return View(new UserIndexView
            {
                Search = string.Empty,
                Users = users,
                Roles = roles,
                IsRolesEnabled = true
            });
        }

        public ViewResult CreateUser()
        {
            var model = InitialRoles();
            return View(model);
        }

        public async Task<ActionResult> CreateRole(string id)
        {
            var role = new IdentityRole(id);

            var result = await RoleManager.CreateAsync(role);
            if (!result.Succeeded)
                ModelState.AddModelError("", result.Errors.First());
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DeleteRole(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            var result = await RoleManager.DeleteAsync(role);
            if (!result.Succeeded)
                ModelState.AddModelError("", result.Errors.First());

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Role(string roleName)
        {
            //List<User> users = WebSecurity.GetUsersInRole(roleName);
            List<ApplicationUser> result = new List<ApplicationUser>();
            var role = await RoleManager.FindByNameAsync(roleName);

            if (role == null)
                return RedirectToAction("Index");

            IEnumerable<IdentityUserRole> users = role.Users;
            foreach (var user in users)
            {
                var applicationUser = await UserManager.FindByIdAsync(user.UserId);
                result.Add(applicationUser);
            }
            ViewBag.RoleName = roleName;
            return View(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserView model)
        {
            var initModel = InitialRoles();
            if (!ModelState.IsValid)
                return View(initModel);
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "使用者密碼輸入有誤，請確認密碼輸入是否正確");
                return View(initModel);
            }
            var emailDadRegister = await UserManager.FindByEmailAsync(model.Email);
            if (emailDadRegister != null)
            {
                ModelState.AddModelError("", "此 Email 已經被使用過，請重新輸入新的 Email");
                return View(initModel);
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                IsApproved = true
            };
            
            var createResult = await UserManager.CreateAsync(user, model.Password);
            if (createResult.Succeeded)
            {
                var roles = model.InitialRoles.Where(p => p.Value == true).Select(s => s.Key).ToList();
                var result = await UserManager.AddUserToRolesAsync(user.Id, roles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(initModel);
                }
            }
            else
            {
                ModelState.AddModelError("", createResult.Errors.First());
                return View(initModel);
            }
            return RedirectToAction("Index");
        }

        private CreateUserView InitialRoles()
        {
            var model = new CreateUserView
            {
                //InitialRoles = roleProvider.GetAllRoles().ToDictionary(k => k, v => false)
                InitialRoles = RoleManager.Roles.Select(s => s.Name).ToDictionary(k => k, v => false)
            };
            return model;
        }

        private IEnumerable<ApplicationUser> GetOnelineUsers(IEnumerable<ApplicationUser> users)
        {
            // 計算 30 分鐘內登入的都算是 Online
            DateTime DateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30.0));
            return users.Where(u => u.LastActivityDate > DateActive);
        }
    }
}