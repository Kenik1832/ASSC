using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using ASSC.Models;
using ASSC.ViewModels;

namespace ASSC.Controllers
{
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

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(model.Role));
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                false,
                false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Dashboard");
            
            // ❗ только здесь ошибка
            TempData["Error"] = "Неверный логин или пароль";
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}