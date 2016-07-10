using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OneTrueError.Client.AspNet.Demo
{
    public partial class Startpage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Complex"] = new {UserName = "Arne", Age = 92};
            Request.Cookies.Add(new HttpCookie("milk", "sour") {Expires = DateTime.Today.AddDays(30)});
        }
    }
}