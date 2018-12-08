using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;



using System.Web.Mvc;





namespace htmltemplate.Models
{
    public class DownloadFiles
    {
        public List<DownLoadFileInformation> GetFiles()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection sql = new SqlConnection(connectionstring);
            string query = string.Format("Select * from [dbo].[upload]");
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
    }
}