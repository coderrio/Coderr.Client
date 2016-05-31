using System.Web.Mvc;

namespace OneTrueError.Client.AspNet.Mvc5.Demo.Controllers
{
    public class Error6Controller : Controller
    {
        public ActionResult Index()
        {
            var model = RouteData.DataTokens["OneTrueModel"];
            return View("Error", model);
        }

        public ActionResult NotFound()
        {
            var model = RouteData.DataTokens["OneTrueModel"];
            return View(model);
        }

        public ActionResult InternalServerError()
        {
            var model = RouteData.DataTokens["OneTrueModel"];
            return View(model);
        }

    }
}