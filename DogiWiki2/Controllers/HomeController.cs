using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using DogiWiki2.Models;

namespace DogiWiki2.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            List<Tuple<string, string, string>> imagesList = new List<Tuple<string, string, string>>();

            string connectionString = "Server=tcp:dogiwikidbserver.database.windows.net,1433;Initial Catalog=DogiWiki_db;Persist Security Info=False;User ID=eric;Password=ek@132EKA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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

            string connectionString = "Server=tcp:dogiwikidbserver.database.windows.net,1433;Initial Catalog=DogiWiki_db;Persist Security Info=False;User ID=eric;Password=ek@132EKA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

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
        public async Task<ActionResult> Upload(HttpPostedFileBase file, UploadModel model)
        {
            Guid guid = Guid.NewGuid();
            String fileNameOfficial = guid.ToString() + ".jpg";
            
            //save the image, resize if it is too large
            try
            {
                Image i = System.Drawing.Image.FromStream(file.InputStream);

                
                var rotate = RotateFlipType.RotateNoneFlipNone;

                //rotate the image before saving if needed
                if (!i.PropertyIdList.Contains(0x112))
                {
                    //do nothing
                }
                else
                {
                    var prop = i.GetPropertyItem(0x112);
                    int val = BitConverter.ToUInt16(prop.Value, 0);

                    if (val == 3 || val == 4)
                        rotate = RotateFlipType.Rotate180FlipNone;
                    else if (val == 5 || val == 6)
                        rotate = RotateFlipType.Rotate90FlipNone;
                    else if (val == 7 || val == 8)
                        rotate = RotateFlipType.Rotate270FlipNone;

                    if (val == 2 || val == 4 || val == 5 || val == 7)
                        rotate |= RotateFlipType.RotateNoneFlipX;
                }

                if (rotate != RotateFlipType.RotateNoneFlipNone)
                    i.RotateFlip(rotate);

                int width = i.Width;
                int height = i.Height;

                int newWidth = 0;
                int newHeight = 0;

                Bitmap m = null;

                if(width > 1200 || height > 1200)
                {
                    if (width > height)
                    {
                        newWidth = 1200;
                        newHeight = 1200 * height / width;
                    }
                    else
                    {
                        newHeight = 1200;
                        newWidth = 1200 * width / height;
                    }

                    m = ResizeImage(i, newWidth, newHeight);
                }
                else
                {
                    m = new Bitmap(i);
                }

                //string path = VirtualPathUtility.ToAbsolute("~/Images");


                ImageCodecInfo jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/jpeg").First();
                using (EncoderParameters encParams = new EncoderParameters(1))
                {
                    encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                    //quality should be in the range [0..100]

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        m.Save(memoryStream, jpgInfo, encParams);
                        memoryStream.Seek(0, SeekOrigin.Begin); // otherwise you'll get zero byte files
                        await UploadModel.WriteBlobStream(memoryStream, "images", fileNameOfficial);
                    }

                    //m.Save(Path.Combine(path, fileNameOfficial+ ".jpg"), jpgInfo, encParams);
                }

                    
                /*
                else
                {
                    string fileName = Path.GetFileName(file.FileName);

                    string extension = Path.GetExtension(fileName);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await file.InputStream.CopyToAsync(memoryStream);
                        //memoryStream.Seek(0, SeekOrigin.Begin); // otherwise you'll get zero byte files
                        await UploadModel.WriteBlobStream(memoryStream, "images", fileNameOfficial + extension, extension);
                    }


                    //string newFileName = guid.ToString() + extension;

                    //string fullPath = Path.Combine(path, newFileName);

                    fileNameOfficial = fileNameOfficial + extension;

                    //file.SaveAs(fullPath);
                }*/
                
            }
            catch(Exception e) { System.Diagnostics.Debug.WriteLine("Error with upload: " + e.Message); }

            //write to database
            try
            {
                string name = model.Name;
                string breed = model.SelectedBreed;
                string description = model.Description;
                string date = DateTime.Now.ToString();
                

                SqlConnection con = new SqlConnection("Server=tcp:dogiwikidbserver.database.windows.net,1433;Initial Catalog=DogiWiki_db;Persist Security Info=False;User ID=eric;Password=ek@132EKA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

                System.Diagnostics.Debug.WriteLine("Connect to db complete");
                con.Open();
                System.Diagnostics.Debug.WriteLine("db open");
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.CommandText = "INSERT INTO [dbo].[Doggos] VALUES ('"+name+"','"+breed+"','"+description+"','"+fileNameOfficial+"','"+date+"',0)";
                cmd.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine("Execute query complete");
                con.Close();
            }
            catch(Exception e) { System.Diagnostics.Debug.WriteLine("Error with writing to database: " + e.Message); }
            


            return Redirect("~/Home/UploadComplete");
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        

    }
}