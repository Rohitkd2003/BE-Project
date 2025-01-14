using BE_PROJECT_1.Models;
using BE_PROJECT_1.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class CompanyController : Controller
    {
        private readonly be_projectEntities _context;

        // Parameterless constructor (only used if DI is not set up)
        public CompanyController()
        {
            _context = new be_projectEntities();  // Initialize the context manually
        }

        // Constructor with DI (optional)
        public CompanyController(be_projectEntities context)
        {
            _context = context;  // Use DI to inject the context
        }

        public ActionResult form()
        {
            return View();
        }

        public ActionResult cgpa()
        {
            return View();
        }


       


        public ActionResult GetPlacementOpportunities(double? studentCgpa, int studentBacklogs, string studentSkills)
        {
            // Check if studentCgpa is null
            if (!studentCgpa.HasValue)
            {
                // Handle the case where studentCgpa is not provided (you can return a view with an error message, etc.)
                ModelState.AddModelError("studentCgpa", "CGPA is required.");
                return View();
            }

            // Proceed with the rest of your logic if studentCgpa is valid
            var companies = _context.Companies.ToList();

            var recommendedCompanies = companies
                .Where(company => studentCgpa >= (double)company.MinCgpa && studentBacklogs <= company.MaxBacklogs)
                .Select(company => new
                {
                    Company = company,
                    FitScore = CalculateFitScore(company, studentCgpa.Value, studentBacklogs, studentSkills)
                })
                .OrderByDescending(x => x.FitScore)
                .Select(x => x.Company)
                .ToList();

            return View(recommendedCompanies);  // Ensure the model is passed correctly
        }



        private double CalculateFitScore(Company company, double studentCgpa, int studentBacklogs, string studentSkills)
        {
            double score = 0;

            // CGPA scoring
            double cgpaScore = (studentCgpa / (double)company.MinCgpa) * 40;  // Weighted 40%

            // Backlogs scoring
            double backlogScore = (studentBacklogs <= company.MaxBacklogs ? 30 : 0);  // Weighted 30%

            // Skill matching (simplified example, actual implementation may vary)
            var requiredSkills = company.RequiredSkills.Split(',');
            var studentSkillsArray = studentSkills.Split(',');
            int skillMatchCount = requiredSkills.Count(skill => studentSkillsArray.Contains(skill));
            double skillScore = ((double)skillMatchCount / requiredSkills.Length) * 30;  // Weighted 30%

            score = cgpaScore + backlogScore + skillScore;
            return score;
        }


        //[HttpPost]
        //public async Task<ActionResult> SubmitApplication(placementcompany model, HttpPostedFileBase resume)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Save resume file
        //        if (resume != null && resume.Length > 0)
        //        {
        //            var filePath = Path.Combine("Content/resumes", resume.FileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await resume.CopyToAsync(stream);
        //            }
        //            model.ResumePath = $"/resumes/{resume.FileName}";
        //        }

        //        // Save model to database
        //        _context.placementcompanies.Add(model);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("ApplicationSuccess");
        //    }
        //    return View(model);
        //}

        //public ActionResult ApplicationSuccess()
        //{
        //    return View();
        //}
    }

    


}
