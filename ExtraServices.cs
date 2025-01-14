using BE_PROJECT_1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace BE_PROJECT_1.Services
{
   

    public class ExtraServices
    {
        //  --> TO GENERATE RANDOM PASSWORD
        public static string GenerateRandomPassword(int length)
        {
            string password = "";
            string data = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            Random r = new Random();
            for (int i = 1; i <= length; i++)
            {
                int p = r.Next(0, data.Length - 1);
                password += data[p];
            }
            return password;
        }
        // --> TO GENERATE NEXT EMPLOYEE
        public static string NextStudentCode()
        {
            be_projectEntities db = new be_projectEntities();
            List<tblstudent> lst = db.tblstudents.ToList();
            string ecode = "";
            int nextid;

            // Filter existing codes for E or TE prefix
            var eCodes = lst.Where(e => e.student_code.StartsWith("BE") && !e.student_code.StartsWith("TE"))
                            .Select(e => e.student_id)
                            .ToList();
            var teCodes = lst.Where(e => e.student_code.StartsWith("TE"))
                             .Select(e => e.student_id)
                             .ToList();

            if (eCodes.Count == 0 && teCodes.Count == 0)
            {
                // Start with TE001 if no codes exist
                ecode = "TE001";
            }
            else if (eCodes.Count == 0)
            {
                // If only TE codes exist, assign the next E code
                ecode = "BE001";
            }
            else if (teCodes.Count == 0)
            {
                // If only E codes exist, assign the next TE code
                ecode = "TE001";
            }
            else
            {
                // Get max ID for E and TE codes separately
                int maxEId = eCodes.Max();
                int maxTEId = teCodes.Max();

                if (maxEId > maxTEId)
                {
                    nextid = maxEId + 1;
                    if (nextid < 10)
                    {
                        ecode = "BE00" + nextid;
                    }
                    else if (nextid >= 10 && nextid < 100)
                    {
                        ecode = "BE0" + nextid;
                    }
                    else
                    {
                        ecode = "BE" + nextid;
                    }
                }
                else
                {
                    nextid = maxTEId + 1;
                    if (nextid < 10)
                    {
                        ecode = "TE00" + nextid;
                    }
                    else if (nextid >= 10 && nextid < 100)
                    {
                        ecode = "TE0" + nextid;
                    }
                    else
                    {
                        ecode = "TE" + nextid;
                    }
                }
            }
            return ecode;
        }


        public static string NextTeacherCode()
        {
            be_projectEntities db = new be_projectEntities();
            List<Teacher> lst = db.Teachers.ToList();
            string ecode = "";
            int nextid;
            if (lst.Count == 0)
            {
                ecode = "T001";
            }
            else
            {
                int maxid = lst.Max(e => e.teacher_id);
                if (maxid > 0 && maxid < 10)
                {
                    nextid = maxid + 1;
                    ecode = "T00" + nextid;
                }
                else if (maxid >= 10 && maxid < 100)
                {
                    nextid = maxid + 1;
                    ecode = "T0" + nextid;

                }
                else if (maxid >= 100 && maxid < 1000)
                {
                    nextid = maxid + 1;
                    ecode = "T" + nextid;
                }
            }
            return ecode;
        }

        public static string GenerateOtp(int size)
        {
            string otp = "";
            string data = "0123456789";
            Random r = new Random();
            for (int i = 1; i <= size; i++)
            {
                int p = r.Next(0, data.Length - 1);
                otp += data[p];
            }
            return otp;
        }

        //public static void Send_Email_With_attachment(string email, string subject, String description)
        //{

        //    var fromAddress = new MailAddress("ciitinstitute63@gmail.com", "CIIT Training Institute");
        //    var toAddress = new MailAddress(email, email);
        //    MailMessage message = new MailMessage(fromAddress, toAddress);
        //    message.Subject = subject;
        //    message.Body = description;
        //    message.IsBodyHtml = true;
        //    message.Attachments.Add(new Attachment("D:\\Desktop 12-March 2023\\Extra Images\\.net Frameworks.png"));

        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Host = "smtp.gmail.com";
        //    smtp.Port = 587;
        //    smtp.EnableSsl = true;

        //    NetworkCredential networkcred = new NetworkCredential();
        //    networkcred.UserName = "ciitinstitute63@gmail.com";
        //    networkcred.Password = "uleyuvqczyahhnzc";
        //    smtp.UseDefaultCredentials = true;
        //    smtp.Credentials = networkcred;

        //    smtp.Send(message);
        //}

        public static void Send_Email(string email, string subject, String description)
        {

            var fromAddress = new MailAddress("rohitkd2003@gmail.com", "ROHIT DESHMUKH");
            var toAddress = new MailAddress(email, email);
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = description;
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential networkcred = new NetworkCredential();
            networkcred.UserName = "rohitkd2003@gmail.com";
            networkcred.Password = "jzrgtfkwfnayeimt";
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkcred;

            smtp.Send(message);
        }
    }
}
