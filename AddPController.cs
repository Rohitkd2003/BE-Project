using System.Web.Mvc;
using BE_PROJECT_1.Models; // Replace with your actual namespace
using System.Data.Entity;
using System.Linq; // If using Entity Framework

namespace BE_PROJECT_1.Controllers
{
    public class AddCController : Controller
    {
        private be_projectEntities db = new be_projectEntities(); // Replace with your DbContext class

        // GET: Company/Create
        public ActionResult add()
        {
            return View();
        }

        // POST: PlacementCompany/Create
        [HttpPost]
        public ActionResult add(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);  // Add the entry to the database
                db.SaveChanges();                            // Save the changes to the database
                return RedirectToAction("all", "PlacementCell");            // Redirect to a list or success page
            }

            return View(company);  // If validation fails, return to the form with validation errors
        }
    }
}
