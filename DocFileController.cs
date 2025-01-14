using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BE_PROJECT_1.Models;

namespace BE_PROJECT_1.Controllers
{
    public class DocFileController : Controller
    {
        private readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["be_projectSqlConnection"].ConnectionString;

        // GET: DocFile/Upload
        public ActionResult Upload()
        {
            return View();
        }

        // POST: DocFile/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.Message = "Please select a valid DOC file.";
                return View();
            }

            // Check if the uploaded file is a DOC or DOCX
            if (file.ContentType != "application/msword" && file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                ViewBag.Message = "Only DOC or DOCX files are allowed.";
                return View();
            }

            var docFile = new DocFile
            {
                FileName = Path.GetFileName(file.FileName),
                FileType = file.ContentType
            };

            try
            {
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    docFile.FileData = binaryReader.ReadBytes(file.ContentLength);
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO DocFiles (FileName, FileData, FileType) VALUES (@FileName, @FileData, @FileType)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FileName", docFile.FileName);
                    cmd.Parameters.AddWithValue("@FileData", docFile.FileData);
                    cmd.Parameters.AddWithValue("@FileType", docFile.FileType);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "DOC uploaded successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error uploading DOC file: " + ex.Message;
            }

            return View();
        }

        // GET: DocFile/ViewDoc/5
        public ActionResult ViewDoc(int id)
        {
            DocFile docFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM DocFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            docFile = new DocFile
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
                ViewBag.Message = "Error retrieving DOC file: " + ex.Message;
                return View("Error");
            }

            if (docFile == null)
            {
                return HttpNotFound("DOC file not found.");
            }

            // Setting Content-Disposition as "inline" to view in the browser
            Response.AppendHeader("Content-Disposition", $"inline; filename=\"{docFile.FileName}\"");

            // Using File method to return file data
            return File(docFile.FileData, docFile.FileType);
        }


        // GET: DocFile/DownloadDoc/5
        public ActionResult DownloadDoc(int id)
        {
            DocFile docFile = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT FileName, FileData, FileType FROM DocFiles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            docFile = new DocFile
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
                ViewBag.Message = "Error retrieving DOC file: " + ex.Message;
                return View("Error");
            }

            if (docFile == null)
            {
                return HttpNotFound("DOC file not found.");
            }

            // Return the DOC file for download
            return File(docFile.FileData, docFile.FileType, docFile.FileName);
        }

        // GET: DocFile/Index
        public ActionResult Index()
        {
            var docFiles = new List<DocFile>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, FileName FROM DocFiles"; // Include Id to enable download
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        docFiles.Add(new DocFile
                        {
                            Id = (int)reader["Id"],
                            FileName = reader["FileName"].ToString()
                        });
                    }
                }
            }

            return View(docFiles);
        }
    }
}
