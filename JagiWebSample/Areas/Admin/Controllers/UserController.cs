using Jagi.Helpers;
using Jagi.Utility;
using JagiWebSample.Areas.Admin.Models;
using JagiWebSample.Models;
using Microsoft.AspNet.Identity;
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

            ViewBag.OnlineUsers = GetOnlineUsers(users);

            return View(new UserIndexView
            {
                Search = search,
                Users = users,
                Roles = roles,
                IsRolesEnabled = isRolesEnabled,
                OnlineUsers = GetOnlineUsers(users)
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
            {
                TempData["WarningMessage"] = "新的使用者驗證錯誤，請重新輸入。";
                return View(initModel);
            }
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
                UserName = model.Email,
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
                    TempData["WarningMessage"] = result.Errors.First();
                    return View(initModel);
                }
            }
            else
            {
                ModelState.AddModelError("", createResult.Errors.First());
                TempData["WarningMessage"] = createResult.Errors.First();
                return View(initModel);
            }
            TempData["SuccessMessage"] = "新增使用者成功！";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string userName)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded) {
                    ModelState.AddModelError("", result.Errors.First());
                    TempData["WarningMessage"] = result.Errors.First();
                }
                else
                {
                    TempData["SuccessMessage"] = "刪除使用者成功！";
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<ViewResult> Details(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new DetailsViewModel
            {
                DisplayName = user.UserName,
                User = user,
                Roles = RoleManager.Roles.Select(s => s.Name).ToDictionary(k => k, k => userRoles.Contains(k)),
                Status = !user.IsApproved
                                ? DetailsViewModel.StatusEnum.Unapproved
                                : DetailsViewModel.StatusEnum.Offline
            });
        }

        [HttpPost]
        public async Task<ActionResult> ChangeMail(string id, string email, string title, string comments)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null && user.Id != id)
            {
                TempData["WarningMessage"] = "此 Email " + email + " 已經被使用過，請重新輸入新的 Email";
                return RedirectToAction("Details", new { id });
            }

            user = await UserManager.FindByIdAsync(id);

            if (string.IsNullOrEmpty(title))
                title = "Email From Server";

            if (user.Email != email)
            {
                user.Email = email;
                title = "修改使用者預設郵件";
                comments = comments + "\n修改 email 由 {0} 改為： {1}".FormatWith(user.Email, email);
                await UserManager.UpdateAsync(user);
            }
            comments = comments + "\n\n ==============================";
            comments = comments + "\n 此封郵件為系統自動產生，若有問題請跟管理員聯繫。請勿直接回傳";
            Mailer mailer = new Mailer();
            await mailer.SendGMail(email, comments, title);

            TempData["SuccessMessage"] = "郵件發送成功！";
            return RedirectToAction("Details", new { id });
        }

        public async Task<ViewResult> Password(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new DetailsViewModel
            {
                DisplayName = user.UserName,
                User = user,
                Roles = RoleManager.Roles.Select(s => s.Name)
                    .ToDictionary(role => role, role => userRoles.Contains(role)),

                Status = !user.IsApproved
                                ? DetailsViewModel.StatusEnum.Unapproved
                                    : DetailsViewModel.StatusEnum.Offline
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult SetPassword(string id, string newPassword)
        {
            var token = UserManager.GeneratePasswordResetToken(id);
            var result = UserManager.ResetPassword(id, token, newPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "使用者密碼更改成功！";
                UserManager.UpdateSecurityStamp(id);
            }
            else
            {
                TempData["WarningMessage"] = result.Errors.First();
            }
            return RedirectToAction("Password", new { id });
        }

        [HttpPost]
        public RedirectToRouteResult ChangeApproval(string id, bool isApproved)
        {
            var user = UserManager.FindById(id);
            user.IsApproved = isApproved;
            UserManager.Update(user);
            return RedirectToAction("Details", new { id });
        }

        public ViewResult UsersRoles(string id)
        {
            var user = UserManager.FindById(id);
            var userRoles = UserManager.GetRoles(user.Id);
            return View(new DetailsViewModel
            {
                DisplayName = user.UserName,
                User = user,
                Roles = RoleManager.Roles.Select(s => s.Name).ToDictionary(role => role, role => userRoles.Contains(role)),

                Status = !user.IsApproved
                                ? DetailsViewModel.StatusEnum.Unapproved
                                    : DetailsViewModel.StatusEnum.Offline
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<RedirectToRouteResult> AddToRole(string id, string role)
        {
            string[] roles = { role };
            var result = await UserManager.AddUserToRolesAsync(id, roles);
            return RedirectToAction("UsersRoles", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult RemoveFromRole(string id, string role)
        {
            var result = UserManager.RemoveFromRole(id, role);
            return RedirectToAction("UsersRoles", new { id });
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

        private IEnumerable<ApplicationUser> GetOnlineUsers(IEnumerable<ApplicationUser> users)
        {
            // 計算 30 分鐘內登入的都算是 Online
            DateTime DateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(30.0));
            return users.Where(u => u.LastActivityDate > DateActive);
        }
    }
}