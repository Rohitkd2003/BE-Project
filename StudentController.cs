using BE_PROJECT_1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE_PROJECT_1.Services;

namespace BE_PROJECT_1.Controllers
{
    public class StudentController : Controller
    {
        be_projectEntities db;

        public StudentController()
        {
            db = new be_projectEntities();
        }

        public ActionResult Apiform()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apiform(jobapi ct)
        {
            if (ModelState.IsValid)
            {
                ct.SubmittedAt = DateTime.Now;
                db.jobapis.Add(ct);
                db.SaveChanges();
                return Redirect("https://localhost:7148/Job/jobs");
            }
            return View(ct);
        }

        public ActionResult jobform1()
        {
            return View();
        }

        public ActionResult StudentLayout()
        {
            return View();
        }

        public ActionResult StudentLayoutTE()
        {
            return View();
        }

        public ActionResult jobform()
        {
            return View();
        }

        public ActionResult jobform2()
        {
            return View();
        }

        public ActionResult job()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        public ActionResult job(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
            }
            return RedirectToAction("cgpa","Company");

            //return RedirectToAction("jobform", "Student");
        }

        public ActionResult Index()
        {
            List<tblstudent> lst = db.tblstudents.ToList();
            return View(lst);
        }

        public ActionResult Create()
        {
            string empcode = ExtraServices.NextStudentCode();
            ViewBag.Designations = GetDesignations();
            tblstudent e = new tblstudent() { student_code = empcode };
            return View(e);
        }

        [HttpPost]
        public ActionResult Create(tblstudent emp, HttpPostedFileBase photo)
        {
            string imgname = emp.student_code + Path.GetExtension(photo.FileName);
            string imgpath = Server.MapPath("~/Student Photo/" + imgname);
            photo.SaveAs(imgpath);
            emp.profile_photo = imgname;
            string password = ExtraServices.GenerateRandomPassword(10);
            emp.password = password;
            Session["student"] = emp;
            string otp = ExtraServices.GenerateOtp(6);
            Session["session_otp"] = otp;
            string msg = "<h2>Dear " + emp.student_name + ",</h2><p>Your account registration otp is <b>" + otp + "</b></p><br/><br/><br/><br/><h5>Regards,</h5><h5>SAE</h5>";
            ExtraServices.Send_Email(emp.email_address, "Email Confirmation OTP", msg);
            return RedirectToAction("ConfirmOtp");
        }

        public ActionResult ConfirmOtp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmOtp(string user_otp)
        {
            string otp = Session["session_otp"].ToString();
            if (user_otp == otp)
            {
                tblstudent e = (tblstudent)Session["student"];
                db.tblstudents.Add(e);
                db.SaveChanges();
                string msg = "Dear <h2>" + e.student_name + ",</h2><p>Your account has been created successfully.</p><p>You can access your account by using following credentials</p><p>Student Code=<b>" + e.student_code + "</b></p><p>Password=<b>" + e.password + "</b></p><br/><br/><h2>Regards,<br/>SAE</h2>";
                ExtraServices.Send_Email(e.email_address, "Account Confirmation", msg);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.msg = "Invalid OTP";
                return View();
            }
        }

        public List<SelectListItem> GetDesignations()
        {
            List<SelectListItem> lst = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Select Stream",
                    Value = "0",
                    Selected = true,
                    Disabled = true
                }
            };
            foreach (tblstream d in db.tblstreams.ToList())
            {
                lst.Add(new SelectListItem
                {
                    Text = d.stream_name,
                    Value = d.stream_id.ToString()
                });
            }
            return lst;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string prefix, string student_code, string password)
        {
            string fullStudentCode = prefix + student_code;

            tblstudent e = db.tblstudents
                .FirstOrDefault(p => p.student_code.Equals(fullStudentCode) && p.password.Equals(password));

            if (e != null)
            {
                Session["emp"] = e;
                string otp = ExtraServices.GenerateOtp(6);
                string msg = "Dear <h2>" + e.student_name + ",</h2><p>Your Login OTP is <b>" + otp + "</b></p>";
                Session["otp"] = otp;
                ExtraServices.Send_Email(e.email_address, "Login OTP Confirmation", msg);

                if (fullStudentCode.StartsWith("BE0"))
                {
                    return RedirectToAction("Confirm_LoginOTP");
                }
                else if (fullStudentCode.StartsWith("TE0"))
                {
                    return RedirectToAction("Confirm_LoginOTPTE");
                }
            }
            else
            {
                ViewBag.msg = "Invalid Student Code or Password";
                return View();
            }

            return RedirectToAction("DefaultPage", "YourControllerName");
        }

        public ActionResult Confirm_LoginOTP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Confirm_LoginOTP(string login_otp)
        {
            string otp = Session["otp"].ToString();
            if (otp == login_otp)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid OTP";
                return View();
            }
        }

        public ActionResult Confirm_LoginOTPTE()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Confirm_LoginOTPTE(string login_otp)
        {
            string otp = Session["otp"].ToString();
            if (otp == login_otp)
            {
                return RedirectToAction("DashboardTE");
            }
            else
            {
                ViewBag.msg = "Invalid OTP";
                return View();
            }
        }



        public ActionResult Dashboard()
        {
            tblstudent e = (tblstudent)Session["emp"];
            return View(e);
        }


        public ActionResult DashboardTE()
        {
            tblstudent e = (tblstudent)Session["emp"];
            return View(e);
        }




        public ActionResult ChangePassword()
        {
            if (Session["emp"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                tblstudent e = (tblstudent)Session["emp"];
                return View(e);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string old_password, string new_password, string confirm_password)
        {
            if (Session["emp"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                tblstudent e = (tblstudent)Session["emp"];
                if (e.password == old_password)
                {
                    if (new_password == confirm_password)
                    {
                        tblstudent emp = db.tblstudents.Find(e.student_id);
                        emp.password = new_password;
                        db.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["emp"] = null;
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.msg = "New Password & Confirm Password are not matched";
                        return View(e);
                    }
                }
                else
                {
                    ViewBag.msg = "Old Password is not matched";
                    return View(e);
                }
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email_Address)
        {
            tblstudent e = db.tblstudents.FirstOrDefault(p => p.email_address.Equals(Email_Address));
            if (e != null)
            {
                string url = "https://localhost:44337/Student/NewPassword/" + e.student_id;
                string msg = "Click Below For Login <br/> <a href='" + url + "' target='_blank'>" + url + "</a>";
                ExtraServices.Send_Email(e.email_address, "Generate New Password", msg);
                ViewBag.msg = "Check your email to generate a new password";
                return View();
            }
            else
            {
                ViewBag.msg = "No Employee with given email address is present";
                return View();
            }
        }

        public ActionResult NewPassword(int id)
        {
            tblstudent e = db.tblstudents.Find(id);
            return View(e);
        }

        [HttpPost]
        public ActionResult NewPassword(int student_id, string new_password, string confirm_password)
        {
            tblstudent e = db.tblstudents.Find(student_id);
            if (new_password == confirm_password)
            {
                e.password = new_password;
                db.Entry(e).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.msg = "New Password & Confirm Password are not matched";
                return View(e);
            }
        }
    }
}
