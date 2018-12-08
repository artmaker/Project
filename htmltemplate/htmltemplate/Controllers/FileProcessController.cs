using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using htmltemplate.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace htmltemplate.Controllers
{
    public class FileProcessController : Controller
    {
        // GET: FileProcess
        DownloadFiles obj;
        public FileProcessController()
        {
            obj = new DownloadFiles();
        }
        public List<DownLoadFileInformation> GetFiles()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[upload] where uploadedby='{0}'",User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            while (reader.Read())
            {
                DownLoadFileInformation down = new DownLoadFileInformation();
                down.FileId = Convert.ToInt32(reader["id"]);
                down.FileName = reader["FileName"].ToString();
                down.FilePath = reader["Filepath"].ToString();
                down.Description = reader["Descrip"].ToString();
                lstFiles.Add(down);
            }

            
            return lstFiles;

        }


        public List<DownLoadFileInformation> GetStudentFiles(string idea)
        {
            List<DownLoadFileInformation> lstFiles = new List<DownLoadFileInformation>();
            if (idea != null)
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection sql = new SqlConnection(connectionstring);
                string query = string.Format("Select * from [dbo].[upload] where IdeaId='{0}'", idea);
                SqlCommand sqlcommand = new SqlCommand(query, sql);
                sql.Open();
                SqlDataReader reader = sqlcommand.ExecuteReader();


                while (reader.Read())
                {
                    DownLoadFileInformation down = new DownLoadFileInformation();
                    down.FileId = Convert.ToInt32(reader["id"]);
                    down.FileName = reader["FileName"].ToString();
                    down.FilePath = reader["Filepath"].ToString();
                    down.Description = reader["Descrip"].ToString();
                    lstFiles.Add(down);
                }
            }


            return lstFiles;

        }

        public ActionResult Index()
        {
            //var filesCollection = obj.GetFiles();
            var filesCollection = GetFiles();

            return View(filesCollection);
        }
        public ActionResult StudentsIndex()
        {
            //var filesCollection = obj.GetFiles();
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string sequery = string.Format("Select * from [dbo].[RequestedIdeas] where AppliedBy='{0}' and Approved=1", User.Identity.Name);
            SqlCommand sesqlcommand = new SqlCommand(sequery, sql);
            sql.Open();
            SqlDataReader sereader = sesqlcommand.ExecuteReader();

            RequestedIdeas up = new RequestedIdeas();
            while (sereader.Read())
            {

                up.AppliedBy = sereader["AppliedBy"].ToString();
                up.IdeaId = sereader["IdeaId"].ToString();
                up.RequestTo = sereader["RequestTo"].ToString();


            }
            sql.Close();
            var filesCollection = GetStudentFiles(up.IdeaId);

            return View(filesCollection);
        }

        public FileResult Download(string FileID)
        {
            int CurrentFileID = Convert.ToInt32(FileID);
            
            var filesCol = obj.GetFiles();
            string CurrentFileName = (from fls in filesCol
                                      where fls.FileId == CurrentFileID
                                      select fls.FilePath).First();

            string contentType = string.Empty;

            if (CurrentFileName.Contains(".pdf"))
            {
                contentType = "application/pdf";
            }

            else if (CurrentFileName.Contains(".docx"))
            {
                contentType = "application/docx";
            }
            return File(CurrentFileName, contentType, CurrentFileName);
        }
    }
}