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
    public class TeacherController : Controller
    {
        be_projectEntities db;

        public TeacherController()
        {
            db = new be_projectEntities();
        }


        public ActionResult StudyMaterial()
        {
            return View();
        }

        public ActionResult TeacherLayout()
        {
            return View();
        }


        public ActionResult Index()
        {
            List<Teacher> lst = db.Teachers.ToList();
            return View(lst);
        }

        public ActionResult Create()
        {
            string empcode = ExtraServices.NextTeacherCode();
            //ViewBag.Designations = GetDesignations();
            Teacher e = new Teacher() { teacher_code = empcode };
            return View(e);
        }

        //--> FOR ADDING PHOTO,RANDOM
        [HttpPost]
        public ActionResult Create(Teacher emp, HttpPostedFileBase photo)
        {
            string imgname = emp.teacher_code + Path.GetExtension(photo.FileName);
            string imgpath = Server.MapPath("~/Teacher Photo/" + imgname);
            photo.SaveAs(imgpath);
            emp.profile_photo = imgname;
            string password = ExtraServices.GenerateRandomPassword(10);
            emp.password = password;
            Session["teacher"] = emp;
            string otp = ExtraServices.GenerateOtp(6);
            Session["session_otp"] = otp;
            string msg = "<h2>Dear " + emp.teacher_name + ",</h2><p>Your account registration otp is <b>" + otp + "</b></p><br/><br/><br/><br/><h5>Regards,</h5><h5>SAE</h5>";
            ExtraServices.Send_Email(emp.email_address, "Email Confirmation OTP", msg);
            return RedirectToAction("ConfirmOtp");
            //db.tblemployees.Add(emp);
            //db.SaveChanges();
            //ViewBag.msg = "Employee Added Successfully";
            //ModelState.Clear();
            //string empcode = ExtraServices.NextEmployeeCode();
            //ViewBag.Designations = GetDesignations();
            //tblemployee e = new tblemployee() { employee_code = empcode };
            //return View(e);
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
                Teacher e = (Teacher)Session["teacher"];
                db.Teachers.Add(e);
                db.SaveChanges();
                string msg = "Dear <h2>" + e.teacher_name + ",</h2><p>Your account has been created successfully.</p><p>You can access your account by using following credentials</p><p>Student Code=<b>" + e.teacher_code + "</b></p><p>Password=<b>" + e.password + "</b></p><br/><br/><h2>Regards,<br/>SAE</h2>";
                ExtraServices.Send_Email(e.email_address, "Account Confirmation", msg);
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.msg = "Invalid OTP";
                return View();

            }
        }
        // --> FOR DROP DOWN MENU
        //public List<SelectListItem> GetDesignations()
        //{
        //    List<SelectListItem> lst = new List<SelectListItem>();
        //    SelectListItem st = new SelectListItem()
        //    {
        //        Text = "Select Stream",
        //        Value = "0",
        //        Selected = true,
        //        Disabled = true
        //    };
        //    lst.Add(st);
        //    foreach (tblstream d in db.tblstreams.ToList())
        //    {
        //        SelectListItem s = new SelectListItem
        //        {
        //            Text = d.stream_name,
        //            Value = d.stream_id.ToString()
        //        };
        //        lst.Add(s);
        //    }
        //    return lst;
        //}


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string teacher_code, string password)
        {
            Teacher e = db.Teachers.ToList().FirstOrDefault(p => p.teacher_code.Equals(teacher_code) & p.password.Equals(password));
            if (e != null)
            {
                Session["emp"] = e;
                string otp = ExtraServices.GenerateOtp(6);
                string msg = "Dear <h2>" + e.teacher_name + ",</h2><p>Your Login OTP is <b>" + otp + "</b></p>";
                Session["otp"] = otp;
                ExtraServices.Send_Email(e.email_address, "Login OTP Confirmation", msg);
                return RedirectToAction("Confirm_LoginOTP");
            }
            else
            {
                ViewBag.msg = "Invalid Teacher Code or Password";
                return View();

            }
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
        public ActionResult Dashboard()
        {
            Teacher e = (Teacher)Session["emp"];
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
                Teacher e = (Teacher)Session["emp"];
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
                Teacher e = (Teacher)Session["emp"];
                if (e.password == old_password)
                {
                    if (new_password == confirm_password)
                    {

                        Teacher emp = db.Teachers.Find(e.teacher_id);
                        emp.password = new_password;
                        db.Entry<Teacher>(emp).State = System.Data.Entity.EntityState.Modified;
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
            Teacher e = db.Teachers.ToList().FirstOrDefault(p => p.email_address.Equals(Email_Address));
            if (e != null)
            {
                string url = "https://localhost:44337/Teacher/NewPassword/" + e.teacher_id;
                string msg = "Click Below For Login <br/> <a href='" + url + "' target='_blank'>" + url + "</a>";
                ExtraServices.Send_Email(e.email_address, "Generate New Pasword", msg);
                ViewBag.msg = "Check your email to password generate new password";
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
            Teacher e = db.Teachers.Find(id);
            return View(e);
        }
        [HttpPost]
        public ActionResult NewPassword(int teacher_id, string new_password, string confirm_password)
        {
            Teacher e = db.Teachers.Find(teacher_id);

            if (new_password == confirm_password)
            {
                e.password = new_password;
                db.Entry<Teacher>(e).State = System.Data.Entity.EntityState.Modified;
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
