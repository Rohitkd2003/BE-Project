using BE_PROJECT_1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class PdfFileController : Controller
    {
        // Use the connection string from the web.config
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["be_projectSqlConnection"].ConnectionString;

        // GET: PdfFile/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: PdfFile/Upload
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect against CSRF
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.Message = "Please select a valid PDF file.";
                return View();
            }

            if (file.ContentType != "application/pdf")
            {
                ViewBag.Message = "Only PDF files are allowed.";
                return View();
            }

            var pdfFile = new PdfFile
            {
                FileName = Path.GetFileName(file.FileName),
                FileType = file.ContentType
            };

            try
            {
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    pdfFile.FileData = binaryReader.ReadBytes(file.ContentLength);
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO PdfFiles (FileName, FileData, FileType) VALUES (@FileName, @FileData, @FileType)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FileName", pdfFile.FileName);
                    cmd.Parameters.AddWithValue("@FileData", pdfFile.FileData);
                    cmd.Parameters.AddWithValue("@FileType", pdfFile.FileType);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "PDF uploaded successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error uploading PDF file: " + ex.Message;
            }

            return View();
        }

        // GET: PdfFile/ViewPdf/5
        public ActionResult ViewPdf(int id)
        {
            PdfFile pdfFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM PdfFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pdfFile = new PdfFile
                            {
                                FileName = reader["FileName"].ToString(),
                                FileData = (byte[])reader["FileData"],
                                FileType = reader["FileType"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error retrieving PDF file: " + ex.Message;
                return View("Error");
            }

            if (pdfFile == null)
            {
                return HttpNotFound("PDF file not found.");
            }

            // Set the Content-Disposition to inline
            Response.AppendHeader("Content-Disposition", $"inline; filename={pdfFile.FileName}");
            return File(pdfFile.FileData, pdfFile.FileType);
        }

        // GET: PdfFile/DownloadPdf/5
        public ActionResult DownloadPdf(int id)
        {
            PdfFile pdfFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM PdfFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pdfFile = new PdfFile
                            {
                                FileName = reader["FileName"].ToString(),
                                FileData = (byte[])reader["FileData"],
                                FileType = reader["FileType"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error retrieving PDF file: " + ex.Message;
                return View("Error");
            }

            if (pdfFile == null)
            {
                return HttpNotFound("PDF file not found.");
            }

            // Return the PDF file for download
            return File(pdfFile.FileData, pdfFile.FileType, pdfFile.FileName);
        }

        // GET: PdfFile/Index
        public ActionResult Index()
        {
            var pdfFiles = new List<PdfFile>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, FileName FROM PdfFiles"; // Include Id to enable download
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pdfFiles.Add(new PdfFile
                        {
                            Id = (int)reader["Id"], // Assuming Id is an int in your PdfFile model
                            FileName = reader["FileName"].ToString()
                        });
                    }
                }
            }

            return View(pdfFiles);
        }
    }
}
