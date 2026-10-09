using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NextGen.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Registration() // view reamin
        {
            return View();
        }
        public ActionResult Login() //remain view
        {
            return View();
        }
        public ActionResult Batches()
        {
            return View();
        }
        
    }
}