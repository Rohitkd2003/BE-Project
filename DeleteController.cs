using System.Linq;
using System.Web.Mvc;
using BE_PROJECT_1.Models;

namespace BE_PROJECT_1.Controllers
{
    public class DeleteController : Controller
    {
        private be_projectEntities db = new be_projectEntities(); // Your DbContext class

        // GET: AddP/Delete
        public ActionResult Delete()
        {
            var companies = db.Companies.ToList(); // Fetch all companies from the database
            return View(companies); // Return the list to the view
        }

        // POST: AddP/Delete
        [HttpPost]
        public ActionResult Delete(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                ModelState.AddModelError("", "Please select a company to delete.");
                var companies = db.Companies.ToList();
                return View(companies); // If no company is selected, return to the form
            }

            var company = db.Companies.FirstOrDefault(c => c.Name == companyName);
            if (company != null)
            {
                db.Companies.Remove(company); // Remove the company
                db.SaveChanges(); // Save changes to the database
            }

            return RedirectToAction("all", "PlacementCell"); // Redirect back to the list of companies
        }
    }
}
