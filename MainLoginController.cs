using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class MainLoginController : Controller
    {
        // GET: MainLogin
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult home()
        {
            return View();
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult officers()
        {
            return View();
        }
        public ActionResult examination()
        {
            return View();
        }
        public ActionResult section()
        {
            return View();
        }
        public ActionResult facilities()
        {
            return View();
        }
        public ActionResult academics()
        {
            return View();
        }
        public ActionResult gallery()
        {
            return View();
        }
        public ActionResult contactus()
        {
            return View();
        }
        public ActionResult loginbutton()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                // If no role is selected, stay on the login page
                return RedirectToAction("Index");
            }

            // Redirect based on the role
            switch (role.ToLower())
            {
                case "teacher":
                    return RedirectToAction("TeacherDashboard");
                case "admin":
                    return RedirectToAction("AdminDashboard");
                case "student":
                    return RedirectToAction("StudentDashboard"); // Fix redirection here
                case "superuser":
                    return RedirectToAction("SuperUserDashboard");
                default:
                    return RedirectToAction("Index");
            }
        }

        public ActionResult TeacherDashboard()
        {
            return View();
        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult StudentDashboard()
        {
            return View();
        }

        public ActionResult SuperUserDashboard()
        {
            return View();
        }
    }
}
