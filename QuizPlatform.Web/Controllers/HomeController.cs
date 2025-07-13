using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuizPlatform.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                    return RedirectToAction("Index", "Admin");

                else if (User.IsInRole("user"))
                    return RedirectToAction("Index", "Quiz");
            }

            return View(); 
        }
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            Response.StatusCode = 403; 
            return View();
        }


    }
}
