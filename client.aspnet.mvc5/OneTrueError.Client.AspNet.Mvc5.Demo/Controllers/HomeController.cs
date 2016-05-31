using System;
using System.Web.Mvc;

namespace OneTrueError.Client.AspNet.Mvc5.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TempData["Transaction"] = new
            {
                Amount = 20000,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            //throw new UnauthorizedAccessException();

            //throw new InvalidOperationException("Failed to increase salary, overflow.");

            return View();
        }
    }
}