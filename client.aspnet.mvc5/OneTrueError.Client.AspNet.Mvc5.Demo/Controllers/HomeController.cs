using System;
using System.IO;
using System.Web.Mvc;
using OneTrueError.Client.Contracts;

namespace OneTrueError.Client.AspNet.Mvc5.Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SimulatedFailure()
        {
            ViewBag.Title = "Hello";
            ViewBag.Model = new
            {
                state = "Running",
                Collected = true
            };

            TempData["DemoKey"] = new
            {
                Amount = 20000,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            //throw new UnauthorizedAccessException();
            try
            {
                throw new InvalidOperationException("Tag demo");
            }
            catch (Exception ex)
            {
                var collection = new ContextCollectionDTO("User");
                collection.Properties.Add("Id", "53338");
                collection.Properties.Add("FirstName", "Jonas");
                collection.Properties.Add("LastName", "Gauffin");
                collection.Properties.Add("UserName", "jgauffin");
                var col2 = new ContextCollectionDTO("ViewModel");
                col2.Properties.Add("Fake", "Make");

                this.ReportException(ex, new [] {collection, col2});
            }
            return View();
        }

        public ActionResult Return()
        {
            throw new InvalidDataException("Unhandled data ex!");
        }
    }
}