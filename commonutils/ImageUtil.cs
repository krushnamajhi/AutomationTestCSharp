using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.commonutils
{
    public class ImageUtil
    {

        private static readonly ILogger logger = LoggerConfig.Logger;


        public void GetEntireScreenshot(string screenshotPath, IWebDriver driver)
        {
            //scrolls to the top of the page
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, -window.scrollHeight)");
            // Get the total size of the page
            var totalWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollWidth"); //documentElement.scrollWidth");
            var totalHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
            // Get the size of the viewport
            var viewportWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.clientWidth"); //documentElement.scrollWidth");
            var viewportHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight"); //documentElement.scrollWidth");

            // We only care about taking multiple images together if it doesn't already fit
            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                try
                {
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    Image img = ScreenshotToImage(screenshot);
                    img.Save(screenshotPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); logger.Here().Error(e.Message);
                    logger.Here().Error(e.StackTrace);
                    logger.Here().Error(e.InnerException.ToString());
                    throw;
                }
            }

            // Split the screen in multiple Rectangles
            var rectangles = SplitScreenToRectangles(totalHeight, viewportHeight, totalWidth, viewportWidth);

            // Build the Image
            var stitchedImage = BuildFullImage(driver, rectangles, totalHeight, viewportHeight, totalWidth, viewportWidth);

            SaveImage(stitchedImage, screenshotPath, ImageFormat.Png);

        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }
            return screenshotImage;
        }

        /// <summary>
        /// This method captures the screenshot of the Current Page
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="path">Location to capture Screenshot</param>
        /// <returns></returns>
        public void CaptureScreenshot(string screenshotPath, IWebDriver driver, string ScreenshotType = "", string ExecutionMode = "")
        {
            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, -window.scrollHeight)");

                if (ScreenshotType.Equals("Desktop", StringComparison.CurrentCultureIgnoreCase))
                {
                    CaptureDesktopScreenshot(screenshotPath);
                }
                else
                {
                    if (ExecutionMode.Equals("Yes", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                        ss.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                    }
                    else
                    {
                        var ss = ((ITakesScreenshot)driver).GetScreenshot();
                        ss.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); logger.Here().Error(e.Message);
                logger.Here().Error(e.StackTrace);
                logger.Here().Error(e.InnerException.ToString());
                throw;
            }
        }

        public void CaptureDesktopScreenshot(String screenshotPath)
        {
            Bitmap bitmap = new Bitmap(1920, 1080);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            SaveImage(bitmap, screenshotPath, ImageFormat.Png);
        }

        private Image BuildFullImage(IWebDriver driver, List<Rectangle> rectangles, int totalHeight, int viewportHeight, int totalWidth, int viewportWidth)
        {
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = System.Drawing.Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                try
                {
                    // Calculate the scrolling (if needed)
                    if (previous != System.Drawing.Rectangle.Empty)
                    {
                        var xDiff = rectangle.Right - previous.Right;
                        var yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                    }
                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    // Build an Image out of the Screenshot
                    var screenshotImage = ScreenshotToImage(screenshot);
                    // Calculate the source Rectangle
                    var sourceRectangle = new System.Drawing.Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                    // Copy the Image
                    using (var graphics = Graphics.FromImage(stitchedImage))
                    {
                        graphics.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                        graphics.Dispose();
                    }
                    // Set the Previous Rectangle
                    previous = rectangle;
                    //Thread.Sleep(300);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); logger.Here().Error(e.Message);
                    logger.Here().Error(e.StackTrace);
                    logger.Here().Error(e.InnerException.ToString());
                    throw;
                }
            }

            return stitchedImage;
        }





        private Image OpenImage(string ImageFilePath)
        {
            FileStream fs = new FileStream(ImageFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Image.FromStream(fs);
        }


        public String ConvertImageToBase64String(String Path)
        {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }


        public String ReduceImageSize_ConvertToBase64(String Path, double reducedTo = 1)
        {
            try
            {
                using (Image source = OpenImage(Path))
                {
                    var AbortCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    Image thumbnail = source.GetThumbnailImage((int)(source.Width * reducedTo), (int)(source.Height * reducedTo), AbortCallback, IntPtr.Zero);
                    using (var memory = new MemoryStream())
                    {
                        thumbnail.Save(memory, source.RawFormat);

                        string base64String = Convert.ToBase64String(memory.ToArray());
                        return base64String;
                    }
                }
            }
            catch (Exception ex)
            {
                //logger.Error("Error on Convertinng File : " + Path);
                //logger.Error("Error:" + ex);
            }
            return "";
        }


        public List<Rectangle> SplitScreenToRectangles(int totalHeight, int viewportHeight, int totalWidth, int viewportWidth)
        {
            var rectangles = new List<System.Drawing.Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                try
                {
                    var newHeight = viewportHeight;
                    // Fix if the height of the element is too big
                    if (y + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - y;
                    }
                    // Loop until the totalWidth is reached
                    for (var x = 0; x < totalWidth; x += viewportWidth)
                    {
                        var newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (x + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - x;
                        }
                        // Create and add the Rectangle
                        var currRect = new System.Drawing.Rectangle(x, y, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); 
                    //logger.Error(e.Message);
                    //logger.Error(e.StackTrace);
                    //logger.Error(e.InnerException);
                    throw e;
                }
            }
            return rectangles;
        }


        public void SaveImage(Image image, String ScreenshotPath, ImageFormat imageFormat)
        {
            Bitmap NewImage = new Bitmap(image);
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    NewImage.Save(memory, ImageFormat.Png);
                    using (FileStream imageStream = File.Create(ScreenshotPath))
                    {
                        memory.WriteTo(imageStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                image.Dispose();
                NewImage.Dispose();
            }
        }

        private static bool ThumbnailCallback()
        {
            return false;
        }


        public MediaEntityModelProvider getImageMedia(String ScreenshotPath)
        {
            if (ScreenshotPath == "")
            {
                throw new Exception("Screenshot Path is Blank, Please provide valid Screenshot Path");
            }
            else
            {
             return MediaEntityBuilder.CreateScreenCaptureFromBase64String(
                    ReduceImageSize_ConvertToBase64(ScreenshotPath,0.6),DateTime.Now.ToString("yyyyMMddhhmmssffff")).Build();
            }
        }

    }
}
