using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Interfaces;

namespace QuizPlatform.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #region Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _authService.Register(model);
            if (response.StatusCode != 200)
            {
                ModelState.AddModelError("", response.Message);
                return View(model);
            }
            return RedirectToAction("Login", "Auth");
        }
        #endregion

        #region Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _authService.Login(model);

            if (response.StatusCode != 200)
            {
                ModelState.AddModelError("", response.Message);
                return View(model);
            }

            var role = response.Data?.GetType().GetProperty("Role")?.GetValue(response.Data, null)?.ToString();

            switch (role?.ToLower().Trim())
            {
                case "admin":
                    return RedirectToAction("Index", "Admin");
                case "user":
                    return RedirectToAction("Index", "Quiz");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region LogOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogOut();
            return RedirectToAction("Login", "Auth");
        }
        #endregion

    }
}
