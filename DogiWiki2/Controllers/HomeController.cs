using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using DogiWiki2.Models;

namespace DogiWiki2.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            List<Tuple<string, string, string>> imagesList = new List<Tuple<string, string, string>>();

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\erock\\Documents\\DogiWiki2\\DogiWiki2\\App_Data\\dogsdb.mdf;Integrated Security=True";
            string queryString = "SELECT * FROM [dbo].[Doggos] ORDER BY DateAdded DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    
                    try
                    {
                        while (reader.Read())
                        {
                            imagesList.Add(Tuple.Create(reader["Name"].ToString(), reader["Description"].ToString(), reader["Filename"].ToString()));
                        }
                    }
                    finally
                    {
                        ViewBag.Images = imagesList;
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error setting list of images: " + e.Message); }

            return View();
        }

        [HttpPost]
        public ActionResult Index(IndexModel model)
        {
            string queryString = "SELECT * FROM [dbo].[Doggos]";
            string where = "";
            string orderBy = "";

            List<Tuple<string, string, string>> imagesList = new List<Tuple<string, string, string>>();

            //set query
            if (model.Filter != "All")
                where = " WHERE Breed='" + model.Filter + "'";

            queryString = queryString + where;

            if (model.SortBy == "Newest")
                orderBy = " ORDER BY DateAdded DESC";
            else if (model.SortBy == "Most Popular")
                orderBy = " ORDER BY BoopCount DESC";

            queryString = queryString + orderBy;

            System.Diagnostics.Debug.WriteLine("Query after filter: " + queryString);

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\erock\\Documents\\DogiWiki2\\DogiWiki2\\App_Data\\dogsdb.mdf;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        while (reader.Read())
                        {
                            imagesList.Add(Tuple.Create(reader["Name"].ToString(), reader["Description"].ToString(), reader["Filename"].ToString()));
                        }
                    }
                    finally
                    {
                        if (model.SortBy == "Random")
                        {
                            //shuffle the list
                            Random rng = new Random();

                            int n = imagesList.Count;
                            while (n > 1)
                            {
                                n--;
                                int k = rng.Next(n + 1);
                                var value = imagesList[k];
                                imagesList[k] = imagesList[n];
                                imagesList[n] = value;
                            }
                        }
                        ViewBag.Images = imagesList;
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error refreshing feed: " + e.Message); }


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult UploadComplete()
        {
            ViewBag.Message = "Upload Complete!";

            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, UploadModel model)
        {
            Guid guid = Guid.NewGuid();
            String fileNameOfficial = "";
            //save the image
            try
            {
                string path = Server.MapPath("~/Images");
                string fileName = Path.GetFileName(file.FileName);

                string extension = Path.GetExtension(fileName);

                string newFileName = guid.ToString() + extension;

                string fullPath = Path.Combine(path, newFileName);

                fileNameOfficial = newFileName;

                file.SaveAs(fullPath);
            }
            catch(Exception e) { System.Diagnostics.Debug.WriteLine("Error with upload: " + e.Message); }

            //write to database
            try
            {
                string name = model.Name;
                string breed = model.SelectedBreed;
                string description = model.Description;
                string date = DateTime.Now.ToString();
                

                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\erock\\Documents\\DogiWiki2\\DogiWiki2\\App_Data\\dogsdb.mdf;Integrated Security=True");

                System.Diagnostics.Debug.WriteLine("Connect to db complete");
                con.Open();
                System.Diagnostics.Debug.WriteLine("db open");
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.CommandText = "INSERT INTO [C:\\Users\\erock\\Documents\\DogiWiki2\\DogiWiki2\\App_Data\\dogsdb.mdf].[dbo].[Doggos] VALUES ('"+name+"','"+breed+"','"+description+"','"+fileNameOfficial+"','"+date+"',0)";
                cmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Execute query complete");
                con.Close();
            }
            catch(Exception e) { System.Diagnostics.Debug.WriteLine("Error with writing to database: " + e.Message); }
            


            return Redirect("~/Home/UploadComplete");
        }

        
    }
}