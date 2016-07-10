using System.Web.Mvc;

namespace OneTrueError.Client.AspNet.Mvc5.Demo.Controllers
{
public class ErrorController : Controller
{
    public ActionResult Index(OneTrueViewModel model)
    {
        return View("Error", model);
    }

    public ActionResult NotFound(OneTrueViewModel model)
    {
        return View(model);
    }

    public ActionResult InternalServerError(OneTrueViewModel model)
    {
        return View(model);
    }

}
}