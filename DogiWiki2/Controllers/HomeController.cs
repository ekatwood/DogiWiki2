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
using Google.Cloud.Vision.V1;
using DogiWiki2.Models;

namespace DogiWiki2.Controllers
{

    public class HomeController : Controller
    {
        private static string connectionString = "Server=tcp:dogiwikidbserver.database.windows.net,1433;Initial Catalog=DogiWiki_db;Persist Security Info=False;User ID=eric;Password=ek@132EKA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public ActionResult Index()
        {
            List<Tuple<string, string, string>> imagesList = new List<Tuple<string, string, string>>();
 
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
                            string name = UploadModel.Base64Decode(reader["Name"].ToString());
                            string desc = "";
                            if (reader["Description"] != null)
                                desc = UploadModel.Base64Decode(reader["Description"].ToString());
                            string filename = reader["Filename"].ToString();

                            imagesList.Add(Tuple.Create(name, desc, filename));
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
                            string name = UploadModel.Base64Decode(reader["Name"].ToString());
                            string desc = "";
                            if (reader["Description"] != null)
                                desc = UploadModel.Base64Decode(reader["Description"].ToString());
                            string filename = reader["Filename"].ToString();

                            imagesList.Add(Tuple.Create(name, desc, filename));
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

            //System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:/temp/My First Project-c3d0567c1ad3.json");

            Guid guid = Guid.NewGuid();
            String fileNameOfficial = guid.ToString() + ".jpg";

            bool imageUploadSuccess = false;
            
            //save the image, resize if it is too large
            try
            {
                System.Drawing.Image i = System.Drawing.Image.FromStream(file.InputStream);

                bool runAwayy = false;

                //check for innaprops stuff
                using (Stream stream = new MemoryStream())
                {
                    i.Save(stream, ImageFormat.Jpeg);
                    stream.Position = 0;
                    Google.Cloud.Vision.V1.Image image = await Google.Cloud.Vision.V1.Image.FromStreamAsync(stream);

                    ImageAnnotatorClient client = ImageAnnotatorClient.Create();
                    SafeSearchAnnotation annotation = client.DetectSafeSearch(image);
                    // Each category is classified as Very Unlikely, Unlikely, Possible, Likely or Very Likely.
                    System.Diagnostics.Debug.WriteLine($"Adult? {annotation.Adult}");
                    System.Diagnostics.Debug.WriteLine($"Spoof? {annotation.Spoof}");
                    System.Diagnostics.Debug.WriteLine($"Violence? {annotation.Violence}");
                    System.Diagnostics.Debug.WriteLine($"Medical? {annotation.Medical}");

                    if(annotation.Adult == Likelihood.Possible || annotation.Adult == Likelihood.Likely || annotation.Adult == Likelihood.VeryLikely 
                        || annotation.Violence == Likelihood.Possible || annotation.Violence == Likelihood.Likely || annotation.Violence == Likelihood.VeryLikely
                        || annotation.Medical == Likelihood.Possible || annotation.Medical == Likelihood.Likely || annotation.Medical == Likelihood.VeryLikely)
                    {
                        runAwayy = true;
                    }
                }

                //return error if pic is inapprop
                if (runAwayy)
                {
                    ViewBag.ErrorMessage = "Our algorithm determined your picture may have inappropriate content. Please choose another.";

                    return View();
                }

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

                        imageUploadSuccess = true;
                    }

                }
                
            }
            catch(Exception e) { 
                System.Diagnostics.Debug.WriteLine("Error with upload: " + e.Message);
                ViewBag.ErrorMessage = "There was an error with your upload. Please try again or select a different picture.";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.CommandText = "INSERT INTO [dbo].[SystemExceptions] VALUES ('Upload Error','" + e.Message + "','" + DateTime.Now.ToString() + "')";
                    cmd.ExecuteNonQuery();

                    connection.Close();
                }

                    return View();
            }

            //write to database
            if (imageUploadSuccess)
            {
                try
                {
                    string name = model.Name;
                    name = UploadModel.Base64Encode(name);
                    string breed = model.SelectedBreed;
                    string description = model.Description;
                    if (String.IsNullOrEmpty(description))
                        description = "";
                    else
                        description = UploadModel.Base64Encode(description);

                    long unixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();


                    SqlConnection con = new SqlConnection(connectionString);

                    con.Open();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.CommandText = "INSERT INTO [dbo].[Doggos] VALUES ('" + name + "','" + breed + "','" + description + "','" + fileNameOfficial + "',0,"+ (int)unixTime+")";
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
                catch (Exception e) { System.Diagnostics.Debug.WriteLine("Error with writing to database: " + e.Message); }

            }



            return Redirect("~/Home/UploadComplete");
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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