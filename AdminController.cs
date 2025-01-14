using BE_PROJECT_1.Models;
using BE_PROJECT_1.Services;
using System.Linq;
using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class AdminController : Controller
    {
        be_projectEntities db;

        public AdminController()
        {
            db = new be_projectEntities();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // Action to open the 'Create New Teacher' page
        public ActionResult CreateTeacher()
        {
            return View();
        }

        // Action to open the 'Create New Student' page
        public ActionResult CreateStudent()
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
            Admin e = db.Admins.ToList().FirstOrDefault(p => p.code.Equals(code) & p.password.Equals(password));
            if (e != null)
            {
                
                return RedirectToAction("Index","Admin");
            }
            else
            {
                ViewBag.msg = "Invalid  Code or Password";
                return View();

            }
        }

    }
}
