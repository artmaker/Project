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
    public class TasksController : Controller
    {
        public List<Tasks> GetTasks(string idea)
        {
            List<Tasks> lstFiles = new List<Tasks>();
            if (idea != null)
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection sql = new SqlConnection(connectionstring);
                string query = string.Format("Select * from [dbo].[Task] where IdeaId='{0}'", idea);
                SqlCommand sqlcommand = new SqlCommand(query, sql);
                sql.Open();
                SqlDataReader reader = sqlcommand.ExecuteReader();


                while (reader.Read())
                {
                    Tasks down = new Tasks();
                    down.TaskId = Convert.ToInt32(reader["Id"]);
                    down.IdeaId = reader["IdeaId"].ToString();
                    down.TasksAssigned = reader["TaskAssigned"].ToString();
                    if (Convert.ToBoolean(reader["TaskCompleted"]) == true)
                    {
                        down.TasksCompleted = "YES";
                    }
                    else
                    {
                        down.TasksCompleted = "NO";
                    }
                    down.Teacher = reader["Teacher"].ToString();
                    down.Title = reader["Title"].ToString();

                    lstFiles.Add(down);
                }
            }

            /*
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/pdf"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            */
            return lstFiles;

        }

        public List<Tasks> GetTeacherTasks()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[Task] where Teacher='{0}'", User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Tasks> lstFiles = new List<Tasks>();
            while (reader.Read())
            {
                Tasks down = new Tasks();
                down.TaskId = Convert.ToInt32(reader["Id"]);
                down.IdeaId = reader["IdeaId"].ToString(); 
                down.TasksAssigned = reader["TaskAssigned"].ToString();
                if (Convert.ToBoolean(reader["TaskCompleted"])==true)
                {
                    down.TasksCompleted = "YES";
                }
                else
                {
                    down.TasksCompleted = "NO";
                }
                
                down.Teacher= reader["Teacher"].ToString(); 
                down.Title= reader["Title"].ToString();


                lstFiles.Add(down);
            }

            /*
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/pdf"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            */
            return lstFiles;

        }
        // GET: Tasks
        public ActionResult Index()
        {
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
            var filecollection = GetTasks(up.IdeaId);

            return View(filecollection);
        }
        public ActionResult TeachersIndex()
        {
            var filecollection = GetTeacherTasks();

            return View(filecollection);
        }
        public List<RequestedIdeas> GetTeacherIdeas()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[RequestedIdeas] where RequestTo='{0}' and Approved=1", User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<RequestedIdeas> lstFiles = new List<RequestedIdeas>();
            while (reader.Read())
            {
                RequestedIdeas down = new RequestedIdeas();
                down.AppliedBy = reader["AppliedBy"].ToString();
                down.IdeaId = reader["IdeaId"].ToString();
                down.IdeaTitle = reader["IdeaTitle"].ToString();
                down.RequestTo = reader["RequestTo"].ToString();

                lstFiles.Add(down);
            }

            /*
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/pdf"));

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new DownLoadFileInformation()
                {

                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            */
            return lstFiles;

        }
        public ActionResult UploadTask()
        {
            var filecollection = GetTeacherIdeas();

            return View(filecollection);
            
        }
        [HttpPost]
        public ActionResult UploadTask(string title,string description,string ideaid)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string thirdquery = "insert into [dbo].[Task] values (@TaskAssigned,@Title,@TaskCompleted,@IdeaId,@Teacher)";
            SqlCommand thirdsqlcommand = new SqlCommand(thirdquery, sql);
            sql.Open();
            thirdsqlcommand.Parameters.AddWithValue("@TaskAssigned", description);
            thirdsqlcommand.Parameters.AddWithValue("@Title", title);
            thirdsqlcommand.Parameters.AddWithValue("@TaskCompleted", 0);
            thirdsqlcommand.Parameters.AddWithValue("@IdeaId", ideaid);
            thirdsqlcommand.Parameters.AddWithValue("@Teacher", User.Identity.Name);




            thirdsqlcommand.ExecuteNonQuery();
            sql.Close();


            return RedirectToAction("TeachersIndex", "Tasks");
           

        }
        public ActionResult Completed(string TaskID)
        {
            int task = Convert.ToInt32(TaskID);
            
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string secondquery = String.Format("update [dbo].[Task] set TaskCompleted={0} where Id={1}",1,task);
            SqlCommand secondsqlcommand = new SqlCommand(secondquery, sql);
            sql.Open();


            secondsqlcommand.ExecuteNonQuery();
            sql.Close();
            return RedirectToAction("TeachersIndex");
        }
    }
}