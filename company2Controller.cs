using BE_PROJECT_1.Models;
using System.Web.Mvc;

public class company2Controller : Controller
{
    private be_projectEntities db = new be_projectEntities(); // Replace with your DbContext class

    // GET: PlacementCompany/Create
    public ActionResult cform2()
    {
        return View();
    }

    // POST: PlacementCompany/Create
    [HttpPost]
    public ActionResult cform2(placementcompany placementCompany)
    {
        if (ModelState.IsValid)
        {
            db.placementcompanies.Add(placementCompany);  // Add the entry to the database
            db.SaveChanges();                            // Save the changes to the database
            return RedirectToAction("jobform1", "Student");            // Redirect to a list or success page
        }

        return View(placementCompany);  // If validation fails, return to the form with validation errors
    }
}
