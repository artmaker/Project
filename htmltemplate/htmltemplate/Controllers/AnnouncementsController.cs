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
    public class AnnouncementsController : Controller
    {
        // GET: Announcements
        public List<Announcements> GetAnnouncements()
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
            sql.Close();





            string query = string.Format("Select * from [dbo].[Announcement] where IdeaId='{0}'",up.IdeaId);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Announcements> lstFiles = new List<Announcements>();
            while (reader.Read())
            {
                Announcements down = new Announcements();
                down.Announce = reader["Announce"].ToString();
                down.Cale= reader["Cale"].ToString();
                down.IdeaId = reader["IdeaId"].ToString();

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
        public List<Announcements> GetTeacherAnnouncements()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[Announcement] where teacher='{0}'", User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Announcements> lstFiles = new List<Announcements>();
            while (reader.Read())
            {
                Announcements down = new Announcements();
                down.Announce = reader["Announce"].ToString();
                down.Cale = reader["Cale"].ToString();
                down.IdeaId = reader["IdeaId"].ToString();

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
        public List<RequestedIdeas> GetTeacherStudentsIdeas()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[RequestedIdeas] where RequestTo='{0}' and Approved=1",User.Identity.Name);
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
        public ActionResult Index()
        {
            var filecollection = GetAnnouncements();

            return View(filecollection);
        }
        public ActionResult TeachersIndex()
        {
            var filecollection = GetTeacherAnnouncements();
            
            return View(filecollection);
        }

        public ActionResult MakeAnnouncement()
        {
            var filecollection = GetTeacherStudentsIdeas();
            if (filecollection == null)
            {
                return RedirectToAction("Index");
            }
            
            ViewBag.Ideas = new SelectList(filecollection, "IdeaId", "IdeaId");
            List<string> ideas = new List<string>();
            foreach (RequestedIdeas item in filecollection)
            {
                ideas.Add(item.IdeaId);
            }
            DropDown drop = new DropDown();
            drop.Ideas = filecollection;
            string ideaid = null;
            drop.idea = ideaid;
            return View(drop);
        }
        [HttpPost]
        public ActionResult MakeAnnouncement(string description,string deadline, string key)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string thirdquery = "insert into [dbo].[Announcement] values (@Announce,@Cale,@IdeaId,@teacher)";
            SqlCommand thirdsqlcommand = new SqlCommand(thirdquery, sql);
            sql.Open();
            thirdsqlcommand.Parameters.AddWithValue("@Announce", description);
            thirdsqlcommand.Parameters.AddWithValue("@Cale", deadline);
            thirdsqlcommand.Parameters.AddWithValue("@IdeaId", key);
            thirdsqlcommand.Parameters.AddWithValue("@teacher", User.Identity.Name);




            thirdsqlcommand.ExecuteNonQuery();
            sql.Close();


            return RedirectToAction("TeachersIndex", "Announcements");
        }
    }
}