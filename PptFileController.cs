using BE_PROJECT_1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace BE_PROJECT_1.Controllers
{
    public class PptFileController : Controller
    {
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["be_projectSqlConnection"].ConnectionString;

        // GET: PptFile/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: PptFile/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.Message = "Please select a valid PPT file.";
                return View();
            }

            // Check if the uploaded file is a PPT or PPTX
            if (file.ContentType != "application/vnd.ms-powerpoint" &&
                file.ContentType != "application/vnd.openxmlformats-officedocument.presentationml.presentation")
            {
                ViewBag.Message = "Only PPT or PPTX files are allowed.";
                return View();
            }

            var pptFile = new PptFile
            {
                FileName = Path.GetFileName(file.FileName),
                FileType = file.ContentType
            };

            try
            {
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    pptFile.FileData = binaryReader.ReadBytes(file.ContentLength);
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO PptFiles (FileName, FileData, FileType) VALUES (@FileName, @FileData, @FileType)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FileName", pptFile.FileName);
                    cmd.Parameters.AddWithValue("@FileData", pptFile.FileData);
                    cmd.Parameters.AddWithValue("@FileType", pptFile.FileType);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "PPT uploaded successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error uploading PPT file: " + ex.Message;
            }

            return View();
        }

        // GET: PptFile/ViewPpt/5
        public ActionResult ViewPpt(int id)
        {
            PptFile pptFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM PptFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pptFile = new PptFile
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
                ViewBag.Message = "Error retrieving PPT file: " + ex.Message;
                return View("Error");
            }

            if (pptFile == null)
            {
                return HttpNotFound("PPT file not found.");
            }

            // Return the PPT file for viewing
            return File(pptFile.FileData, pptFile.FileType, pptFile.FileName);
        }


        // GET: PptFile/DownloadPpt/5
        public ActionResult DownloadPpt(int id)
        {
            PptFile pptFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM PptFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pptFile = new PptFile
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
                ViewBag.Message = "Error retrieving PPT file: " + ex.Message;
                return View("Error");
            }

            if (pptFile == null)
            {
                return HttpNotFound("PPT file not found.");
            }

            // Return the PPT file for download
            return File(pptFile.FileData, pptFile.FileType, pptFile.FileName);
        }

        // GET: PptFile/Index
        public ActionResult Index()
        {
            var pptFiles = new List<PptFile>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, FileName FROM PptFiles"; // Include Id to enable download
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pptFiles.Add(new PptFile
                        {
                            Id = (int)reader["Id"],
                            FileName = reader["FileName"].ToString()
                        });
                    }
                }
            }

            return View(pptFiles);
        }
    }
}
