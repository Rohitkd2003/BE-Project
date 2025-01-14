using BE_PROJECT_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class PlacementCellController : Controller
    {
        be_projectEntities db;

        public PlacementCellController()
        {
            db = new be_projectEntities();
        }
        // GET: PlacementCell
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult all()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string code, string password)
        {
            placement e = db.placements.ToList().FirstOrDefault(p => p.code.Equals(code) & p.password.Equals(password));
            if (e != null)
            {

                return RedirectToAction("all", "PlacementCell");
            }
            else
            {
                ViewBag.msg = "Invalid  Code or Password";
                return View();

            }
        }
    }
}