using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using htmltemplate.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace htmltemplate.Controllers
{
    public class ProgressesController : Controller
    {
        public List<Progress> GetProgress(string idea)
        {
            List<Progress> lstFiles = new List<Progress>();
            if (idea != null)
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection sql = new SqlConnection(connectionstring);
                string query = string.Format("Select * from [dbo].[Progress2] where IdeaId='{0}'", idea);
                SqlCommand sqlcommand = new SqlCommand(query, sql);
                sql.Open();
                SqlDataReader reader = sqlcommand.ExecuteReader();


                while (reader.Read())
                {
                    Progress down = new Progress();
                    down.Bar1 = Convert.ToInt32(reader["Bar1"]);
                    down.Bar2 = Convert.ToInt32(reader["Bar2"]);
                    down.Bar3 = Convert.ToInt32(reader["Bar3"]);
                    down.Bar4 = Convert.ToInt32(reader["Bar4"]);
                    down.IdeaId = idea;


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

        public List<Progress> GetTeacherProgress()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[Progress2] where Teacher='{0}'", User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Progress> lstFiles = new List<Progress>();
            while (reader.Read())
            {
                Progress down = new Progress();
                down.Bar1 = Convert.ToInt32(reader["Bar1"]);
                down.Bar2 = Convert.ToInt32(reader["Bar2"]);
                down.Bar3 = Convert.ToInt32(reader["Bar3"]);
                down.Bar4 = Convert.ToInt32(reader["Bar4"]);
                down.IdeaId = reader["IdeaId"].ToString();
                down.Teacher = reader["Teacher"].ToString();

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
        // GET: Progresses
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
            sql.Close();
            
            var filecollection = GetProgress(up.IdeaId);
            
            return View(filecollection);
        }
        public ActionResult TeachersIndex()
        {
            //var Something = TempData["Something "];
            //ViewBag.Idea = Something;
            var filecollection = GetTeacherProgress();

            return View(filecollection);

        }

        public ActionResult Create()
        {

            return View();
        }

        // POST: Progresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Bar1,Bar2,Bar3,Bar4,IdeaId")] Progress progress)
        {
            if (ModelState.IsValid)
            {

                string connectionstring = ConfigurationManager.ConnectionStrings["HomeEntities"].ToString();
                SqlConnection sql = new SqlConnection(connectionstring);
                string secondquery = String.Format("update [dbo].[Progress] set Bar1={0},Bar2={1},Bar3={2},Bar4={3} where IdeaId={4}",progress.Bar1, progress.Bar2, progress.Bar3, progress.Bar4, progress.IdeaId);
                SqlCommand secondsqlcommand = new SqlCommand(secondquery, sql);
                sql.Open();
                /*
                
                secondsqlcommand.Parameters.AddWithValue("@Bar1",progress.Bar1);

                secondsqlcommand.Parameters.AddWithValue("@Bar2", progress.Bar2);
                secondsqlcommand.Parameters.AddWithValue("@Bar3", progress.Bar3);
                secondsqlcommand.Parameters.AddWithValue("@Bar4", progress.Bar4);
                secondsqlcommand.Parameters.AddWithValue("@IdeaId", progress.IdeaId);
                */

                secondsqlcommand.ExecuteNonQuery();
                sql.Close();
                var Something = progress.IdeaId;
                ViewBag.Idea = Something;

                return RedirectToAction("TeachersIndex", "Progresses");
            }

            return View(progress);
        }
        public ActionResult Createe()
        {
            var filecollection = GetTeacherProgress();

            return View(filecollection);
            
        }
        [HttpPost]
        public ActionResult Createe(string Bar1, string Bar2, string Bar3, string Bar4, string ideaid)
        {
            int bar1 = Convert.ToInt32(Bar1);
            int bar2 = Convert.ToInt32(Bar2);
            int bar3 = Convert.ToInt32(Bar3);
            int bar4 = Convert.ToInt32(Bar4);
            string idea = ideaid.Replace(" ", string.Empty);
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string secondquery = String.Format("update [dbo].[Progress2] set Bar1={0},Bar2={1},Bar3={2},Bar4={3} where IdeaId='{4}'",bar1,bar2,bar3,bar4,idea);
            SqlCommand secondsqlcommand = new SqlCommand(secondquery, sql);
            sql.Open();
            

            secondsqlcommand.ExecuteNonQuery();
            sql.Close();
            

            return RedirectToAction("TeachersIndex", "Progresses");
            

        }
    }
}