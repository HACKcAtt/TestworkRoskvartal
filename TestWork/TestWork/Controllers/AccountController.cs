using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestWork.ViewModels;
using TestWork.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Linq;
using System.Data.Objects;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;
using TestWork.Salter;
using System.Data.SqlClient;


namespace TestWork.Controllers
{
    public class AccountController : Controller
    {
        private DBContext db;

        public AccountController(DBContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            model.wrongUsernameOrPasswordFlag = false;
            return View(model);
        }

        public Users compareUser(string email, string password)
        {
            Users user = db.Users.Include(u => u.UserRoles).FirstOrDefault(u => u.UsersEmail == email && u.UsersPassword == (string)password);
            return (user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string s = Salter.Salter.GetHashString(model.UsersPassword);
                Users user = compareUser(model.UsersEmail, s);
                if (user != null)
                {
                    model.wrongUsernameOrPasswordFlag = false;
                    // Аутентификация.
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }
                model.wrongUsernameOrPasswordFlag = true;
                ModelState.AddModelError("", "Некорректные логин и/или пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()

        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel UsersModel)
        {
            int roleId = db.Roles.Where(r => r.RolesName == "SimpleUser").FirstOrDefault().RolesId;
            Users user = await db.Users.FirstOrDefaultAsync(u => u.UsersEmail == UsersModel.Users.UsersEmail);
            if (user == null)
            {
                user = new Users { UsersEmail = UsersModel.Users.UsersEmail, UsersPassword = Salter.Salter.GetHashString(UsersModel.UsersPassword), UsersName = UsersModel.Users.UsersName, UsersPhoneNumber = UsersModel.Users.UsersPhoneNumber, UsersAddress = UsersModel.Users.UsersAddress, RolesId = roleId, UsersExistedFlag = true };
                Roles userRole = await db.Roles.FirstOrDefaultAsync(r => r.RolesName == "SimpleUser");
                if (userRole != null)
                {
                    user.UserRoles = userRole;
                }

                db.Users.Add(user);
                await db.SaveChangesAsync();

                await Authenticate(user);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и/или пароль");
            }
            return View(UsersModel);
        }

        // Проверка телефона на дупликацию.
        // POST: /CheckInstanceName
        [HttpPost]
        public JsonResult CheckInstanceName(string instanceNameForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = db.Users.Any(s => s.UsersExistedFlag && s.UsersPhoneNumber == instanceNameForCheck);
            // Если результат проверки показал совпадение и профили разные, то вернуть на клиент 1.
            if (comparisonResultFlag)
            {
                return Json(1);
            }
            // Если результат проверки не показал совпадение, или профиль один и тот же, то вернуть на клиент 0.
            else
            {
                return Json(0);
            }
        }

        // Проверка адреса электронной почты на дупликацию.
        // POST: CheckInstanceName
        [HttpPost]
        public JsonResult CheckInstanceEmail(string instanceEmailForCheck, int? instanceIdToPass)
        {
            // Флаг проверки.
            bool comparisonResultFlag = db.Users.Any(s => s.UsersExistedFlag && s.UsersEmail == instanceEmailForCheck);
            if (comparisonResultFlag)
            {
                return Json(1);
            }
            // Если результат проверки не показал совпадение, или профиль один и тот же, то вернуть на клиент 0.
            else
            {
                return Json(0);
            }
        }

        private async Task Authenticate(Users user)
        {
            // Создание одного Claim.
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UsersEmail),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRoles?.RolesName),
                new Claim(ClaimTypes.SerialNumber, user.UsersId.ToString()),
                new Claim(ClaimTypes.StreetAddress, user.UsersAddress),
                new Claim(ClaimTypes.Name, user.UsersName),
                new Claim(ClaimTypes.HomePhone, user.UsersPhoneNumber),
                new Claim(ClaimTypes.DateOfBirth, user.UsersBirthday)
            };
            // Создание объекта ClaimsIdentity.
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // Установка аутентификационных куки.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}