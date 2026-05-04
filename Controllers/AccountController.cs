using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ASSC.Models;
using ASSC.ViewModels;

namespace ASSC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser>
            _signInManager;

        private readonly RoleManager<IdentityRole>
            _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,

            SignInManager<ApplicationUser>
                signInManager,

            RoleManager<IdentityRole>
                roleManager)
        {
            _userManager = userManager;

            _signInManager = signInManager;

            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim();

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(model.Role));
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(
                    user,
                    model.Role);

                // ✅ ВАЖНО
                TempData["Success"] = "Регистрация прошла успешно!";

                return RedirectToAction("Login");
            }

            // ошибки Identity
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim() ?? string.Empty;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["Error"] = "Неверный логин или пароль";
                return View(model);
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                false);

            if (passwordCheck.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Dashboard");
            }

            if (passwordCheck.IsLockedOut)
            {
                TempData["Error"] = "Учетная запись заблокирована. Попробуйте позже.";
                return View(model);
            }

            if (passwordCheck.IsNotAllowed)
            {
                TempData["Error"] = "Вход запрещен. Проверьте настройки учетной записи.";
                return View(model);
            }

            TempData["Error"] = "Неверный логин или пароль";
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}