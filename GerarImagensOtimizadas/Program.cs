
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GerarImagensOtimizadas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string folderWithImagesToResize = @"C:\dados\git\lixo\GerarImagensOtimizadas\bin\Debug\imagesToBeResized";
            string folderToSaveResizedImages = @"C:\dados\git\lixo\GerarImagensOtimizadas\bin\Debug\imagesResized";

            // If there is any TIFF image in the folder, convert it to JPEG
            foreach (var file in Directory.GetFiles(folderWithImagesToResize, "*.tif"))
            {
                ConvertTifToJpg(file, Path.Combine(folderWithImagesToResize, Path.GetFileNameWithoutExtension(file) + ".jpg"));
            }

            // Resize all JPEG images in the folder
            ProcessarJPGImages(folderWithImagesToResize, folderToSaveResizedImages, 800, 600);

            Console.ReadKey();
        }

        static void ProcessarJPGImages(string folderSource, string folderDestination, int maxWidth, int maxHeight)
        {
            foreach (var file in Directory.GetFiles(folderSource, "*.jpg"))
            {
                //var image = ResizeImageProportionally(file, maxWidth, maxHeight);
                //image.Save(Path.Combine(folderDestination, Path.GetFileName(file)));

                var gm = new GraphicsManipulation();

                // Read file as stream
                var ms = new MemoryStream(File.ReadAllBytes(file));
                var stream = gm.CreateThumbnailProportional(ms, 1024, 800);
                System.IO.File.WriteAllBytes(
                    Path.Combine(folderDestination, Path.GetFileName(file)), 
                    ConvertStreamToBytes(stream));
            }
        }

        public static byte[] ConvertStreamToBytes(System.IO.Stream stream)
        {
            byte[] content = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(content, 0, int.Parse(stream.Length.ToString()));
            return content;
        }

        public static void ConvertTifToJpg(string tifFilePath, string jpgFilePath)
        {
            try
            {
                // Load the TIFF image
                Bitmap originalImage = new Bitmap(tifFilePath);

                // Save the TIFF image as a JPEG image
                originalImage.Save(jpgFilePath, ImageFormat.Jpeg);

                // Dispose of the original image
                originalImage.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting TIFF to JPEG: " + ex.Message);
            }
        }

        public static Bitmap ResizeImageProportionally(string filePath, int maxWidth, int maxHeight)
        {
            // Load the image from the file path
            Bitmap originalImage = new Bitmap(filePath);

            // Calculate the new dimensions maintaining the aspect ratio
            double ratio = (double)originalImage.Width / (double)originalImage.Height;
            int newWidth, newHeight;

            if (maxWidth > 0 && maxHeight > 0)
            {
                if (originalImage.Width > originalImage.Height)
                {
                    newWidth = maxWidth;
                    newHeight = (int)(maxWidth * ratio);
                }
                else
                {
                    newHeight = maxHeight;
                    newWidth = (int)(maxHeight * ratio);
                }
            }
            else if (maxWidth > 0)
            {
                newWidth = maxWidth;
                newHeight = (int)(maxWidth * ratio);
            }
            else if (maxHeight > 0)
            {
                newHeight = maxHeight;
                newWidth = (int)(maxHeight * ratio);
            }
            else
            {
                throw new ArgumentException("Either maxWidth or maxHeight must be greater than 0.");
            }

            // Resize the image to the calculated dimensions
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            // Dispose of the original image
            originalImage.Dispose();

            return resizedImage;
        }
    }
}
