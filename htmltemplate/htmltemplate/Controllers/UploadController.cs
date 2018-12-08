using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using htmltemplate.Models;

namespace htmltemplate.Controllers
{
    public class UploadController : Controller
    {
        // GET: Upload
        //ideas to upload documnts
        public List<RequestedIdeas> getIdeaForTeacher()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[RequestedIdeas] where Approved = 1 and RequestTo='{0}'",User.Identity.Name);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<RequestedIdeas> lstFiles = new List<RequestedIdeas>();
            while (reader.Read())
            {
                RequestedIdeas down = new RequestedIdeas();
                down.AppliedBy = reader["AppliedBy"].ToString();
                down.IdeaId = reader["IdeaId"].ToString();
                down.RequestTo = reader["RequestTo"].ToString();
                down.IdeaTitle = reader["IdeaTitle"].ToString();

                lstFiles.Add(down);
            }
            return lstFiles;
        }
        public ActionResult Index()
        {
            var filecollrction = getIdeaForTeacher();
            return View(filecollrction);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file,string test,string idea)
        {
            string val = test;
            if (file != null)
            {
                string filename = Path.GetFileName(file.FileName);
                if (file.ContentLength < 1000000000)
                {
                    file.SaveAs(Server.MapPath("/pdf/" + filename));
                    string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    SqlConnection sql = new SqlConnection(connectionstring);
                    string query = "insert into [dbo].[upload] values (@Filename,@Filepath,@Descrip,@uploadedby,@IdeaId)";
                    SqlCommand sqlcom = new SqlCommand(query, sql);
                    sql.Open();
                    sqlcom.Parameters.AddWithValue("@FileName", filename);
                    sqlcom.Parameters.AddWithValue("@Filepath","/pdf/"+filename);
                    sqlcom.Parameters.AddWithValue("@Descrip", val);
                    sqlcom.Parameters.AddWithValue("@uploadedby", User.Identity.Name);
                    sqlcom.Parameters.AddWithValue("@IdeaId", idea);
                    sqlcom.ExecuteNonQuery();
                    sql.Close();



                }
            }


            return RedirectToAction("Index","FileProcess");
        }
        
        public ActionResult UploadIdea()
        {

            return View();
        }


        [HttpPost]
        public ActionResult UploadIdea(string key,string title,string description)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = "insert into [dbo].[Idea] values (@IdeaKey,@Title,@Description,@UploadedBy)";
            SqlCommand sqlcom = new SqlCommand(query, sql);
            sql.Open();
            sqlcom.Parameters.AddWithValue("@IdeaKey", key);
            sqlcom.Parameters.AddWithValue("@Title", title);
            
            sqlcom.Parameters.AddWithValue("@Description", description);
            sqlcom.Parameters.AddWithValue("@UploadedBy", User.Identity.Name);

            sqlcom.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("TeachersIndex","Announcements");
        }


        public List<RequestedIdeas> getRequestedIdeas()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = "Select * from [dbo].[RequestedIdeas] where Approved != 1";
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<RequestedIdeas> lstFiles = new List<RequestedIdeas>();
            while (reader.Read())
            {
                RequestedIdeas down = new RequestedIdeas();
                down.AppliedBy = reader["AppliedBy"].ToString();
                down.IdeaId = reader["IdeaId"].ToString();
                down.RequestTo = reader["RequestTo"].ToString();
                down.IdeaTitle = reader["IdeaTitle"].ToString();

                lstFiles.Add(down);
            }
            return lstFiles;
        }
        public List<Idea> GetIdeas()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = "Select * from [dbo].[Idea]";
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Idea> lstFiles = new List<Idea>();
            while (reader.Read())
            {
                Idea down = new Idea();
                down.key = reader["IdeaKey"].ToString();
                down.title = reader["Title"].ToString();
                down.description = reader["Description"].ToString();
                
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

        public List<Idea> GetApprovedIdeas()
        {


            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            //
            string findquery = string.Format("Select * from [dbo].[RequestedIdeas] where AppliedBy='{0}'",User.Identity.Name);
            SqlCommand findsqlcommand = new SqlCommand(findquery, sql);
            sql.Open();
            SqlDataReader ireader = findsqlcommand.ExecuteReader();

            List<RequestedIdeas> requested = new List<RequestedIdeas>();
            List<string> Ideaskey = new List<string>();
            while (ireader.Read())
            {
                RequestedIdeas up = new RequestedIdeas();
                up.IdeaId =ireader["IdeaId"].ToString();
                up.IdeaTitle = ireader["IdeaTitle"].ToString();
                up.RequestTo = ireader["RequestTo"].ToString();
                up.AppliedBy = ireader["AppliedBy"].ToString();
                Ideaskey.Add(up.IdeaId);
                requested.Add(up);

                
            }
            //
            sql.Close();






            string query = "Select * from [dbo].[ApprovedIdea]";
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<Idea> lstFiles = new List<Idea>();
            while (reader.Read())
            {
                Idea down = new Idea();
                down.key = reader["IdeaKey"].ToString();
                down.title = reader["Title"].ToString();
                down.description = reader["Description"].ToString();
                string matchedElement = Ideaskey.Find(x => x.Equals(down.key));
                if (matchedElement == null)
                {
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

        public ActionResult ApproveIdea()
        {
            
            var filesCollection = GetIdeas();

            return View(filesCollection);
        }
        
        public ActionResult Accept(string ide)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[Idea] where IdeaKey='{0}'",ide);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();
            List<Idea> lstFiles = new List<Idea>();
            Idea down = new Idea();
            string teacher=null;
            while (reader.Read())
            {
                
                down.key = reader["IdeaKey"].ToString();
                down.title = reader["Title"].ToString();
                down.description = reader["Description"].ToString();
                teacher = reader["UploadedBy"].ToString();

                lstFiles.Add(down);
            }




            teacher = teacher.Replace(" ", string.Empty);
            sql.Close();
            string deletequery = string.Format("Delete from [dbo].[Idea] where IdeaKey='{0}'", ide);
            SqlCommand deletesqlcommand = new SqlCommand(deletequery, sql);
            sql.Open();
             var row = deletesqlcommand.ExecuteNonQuery();
            sql.Close();

            


            string forthquery = string.Format("Select * from [dbo].[AspNetUsers] where UserName='{0}'", teacher);
            SqlCommand forthsqlcommand = new SqlCommand(forthquery, sql);
            sql.Open();
            SqlDataReader forthreader = forthsqlcommand.ExecuteReader();
            string Teacherid=null;
            string Teachername=null;
            while (forthreader.Read())
            {

                Teacherid= forthreader["TeacherId"].ToString();
                Teachername= forthreader["Name"].ToString();
                 
                

                
            }
            sql.Close();


            string secondquery = "insert into [dbo].[ApprovedIdea] values (@Title,@Description,@IdeaKey,@TeacherAssociated)";
            SqlCommand secondsqlcommand = new SqlCommand(secondquery, sql);
            sql.Open();
            secondsqlcommand.Parameters.AddWithValue("@Title", down.title);

            secondsqlcommand.Parameters.AddWithValue("@Description", down.description);
            secondsqlcommand.Parameters.AddWithValue("@IdeaKey", down.key);
            secondsqlcommand.Parameters.AddWithValue("@TeacherAssociated", teacher);
            secondsqlcommand.ExecuteNonQuery();
            sql.Close();

            string thirdquery = "insert into [dbo].[Teacher] values (@TeacherId,@IdeaId,@IdeaTitle,@TeacherName)";
            SqlCommand thirdsqlcommand = new SqlCommand(thirdquery, sql);
            sql.Open();
            thirdsqlcommand.Parameters.AddWithValue("@TeacherId", Teacherid);

            thirdsqlcommand.Parameters.AddWithValue("@IdeaId", down.key);
            thirdsqlcommand.Parameters.AddWithValue("@IdeaTitle", down.title);
            thirdsqlcommand.Parameters.AddWithValue("@TeacherName", Teachername);
            thirdsqlcommand.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("ApproveIdea");

        }


        public ActionResult Reject(string ide)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string deletequery = string.Format("Delete from [dbo].[Idea] where IdeaKey='{0}'", ide);
            SqlCommand deletesqlcommand = new SqlCommand(deletequery, sql);
            sql.Open();
            var row = deletesqlcommand.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("ApproveIdea");
        }

        public ActionResult Apply(string ide)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[ApprovedIdea] where IdeaKey='{0}'", ide);
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();
            
            Idea down = new Idea();
            string teacher = null;
            while (reader.Read())
            {

                down.key = reader["IdeaKey"].ToString();
                down.title = reader["Title"].ToString();
                down.description = reader["Description"].ToString();
                teacher = reader["TeacherAssociated"].ToString();

                
            }

            sql.Close();

            string thirdquery = "insert into [dbo].[RequestedIdeas] values (@IdeaId,@IdeaTitle,@RequestTo,@AppliedBy,@Approved)";
            SqlCommand thirdsqlcommand = new SqlCommand(thirdquery, sql);
            sql.Open();
            thirdsqlcommand.Parameters.AddWithValue("@IdeaId", down.key);

            thirdsqlcommand.Parameters.AddWithValue("@IdeaTitle", down.title);
            thirdsqlcommand.Parameters.AddWithValue("@RequestTo", teacher);
            thirdsqlcommand.Parameters.AddWithValue("@AppliedBy", User.Identity.Name);
            thirdsqlcommand.Parameters.AddWithValue("@Approved", 0);

            thirdsqlcommand.ExecuteNonQuery();
            sql.Close();


            return RedirectToAction("ApprovedIdeas", "Upload");
            //RedirectT("ApprovedIdeas");
            
        }

        public ActionResult ApprovedIdeas()
        {
            
            var filesCollection = GetApprovedIdeas();

            return View(filesCollection);
            
        }

        public List<UserInformation> GetUsers()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = "Select * from [dbo].[AspNetUsers] where Confirmed=0";
            SqlCommand sqlcommand = new SqlCommand(query, sql);
            sql.Open();
            SqlDataReader reader = sqlcommand.ExecuteReader();

            List<UserInformation> lstFiles = new List<UserInformation>();
            while (reader.Read())
            {
                UserInformation down = new UserInformation();
                down.Name = reader["Name"].ToString();
                down.TeacherId = reader["TeacherId"].ToString();
                down.UserType = reader["UserType"].ToString();
                down.Email = reader["UserName"].ToString();

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
        public ActionResult Admin()
        {
            var filesCollection = GetUsers();

            return View(filesCollection);

        }

        public ActionResult AcceptUser(string ide)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string updatequery = string.Format("update [dbo].[AspNetUsers] set Confirmed=1 where UserName='{0}'", ide);
            SqlCommand updatesqlcommand = new SqlCommand(updatequery, sql);
            sql.Open();
            var row = updatesqlcommand.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("Admin");
        }

        public ActionResult RejectUser(string ide)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string deletequery = string.Format("Delete from [dbo].[AspNetUsers] where UserName='{0}'", ide);
            SqlCommand deletesqlcommand = new SqlCommand(deletequery, sql);
            sql.Open();
            var row = deletesqlcommand.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("Admin");
        }

        public ActionResult RequestedIdea()
        {
            var filecollection = getRequestedIdeas();
            return View(filecollection);
        }

        public ActionResult AwardIdea(string ide,string ideaid,string request)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string updatequery = string.Format("update [dbo].[RequestedIdeas] set Approved=1 where AppliedBy='{0}'", ide);
            SqlCommand updatesqlcommand = new SqlCommand(updatequery, sql);
            sql.Open();
            var row = updatesqlcommand.ExecuteNonQuery();
            sql.Close();

            //Initialize progreess bars
            /*
            string seupdatequery = string.Format("update [dbo].[Progress2] set Bar1=0,Bar2=0,Bar3=0,Bar4=0,IdeaId='{0}',Teacher='{1}' where AppliedBy='{0}' and IdeaId='{1}'", ide, ideaid);
            SqlCommand seupdatesqlcommand = new SqlCommand(seupdatequery, sql);
            sql.Open();
            var serow = seupdatesqlcommand.ExecuteNonQuery();
            sql.Close();
            */
            string thirdquery = "insert into [dbo].[Progress2] values (@Bar1,@Bar2,@Bar3,@Bar4,@IdeaId,@Teacher)";
            SqlCommand thirdsqlcommand = new SqlCommand(thirdquery, sql);
            sql.Open();
            thirdsqlcommand.Parameters.AddWithValue("@Bar1", 0);

            thirdsqlcommand.Parameters.AddWithValue("@Bar2", 0);
            thirdsqlcommand.Parameters.AddWithValue("@Bar3", 0);
            thirdsqlcommand.Parameters.AddWithValue("@Bar4", 0);
            thirdsqlcommand.Parameters.AddWithValue("@IdeaId", ideaid);
            thirdsqlcommand.Parameters.AddWithValue("@Teacher", request);


            thirdsqlcommand.ExecuteNonQuery();
            sql.Close();

            return RedirectToAction("RequestedIdea");
        }


    }
}