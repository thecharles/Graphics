using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;

namespace GerarImagensOtimizadas
{
    public class GraphicsManipulation
    {
        //public ImageCodecInfo GetEncoderInfo(string mimeType)
        //{
        //    ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
        //    foreach (ImageCodecInfo item in encoders)
        //        if (item.MimeType == mimeType)
        //            return item;
        //    throw new Exception("ImageCodecInfo not found");
        //}

        public Stream SetJpgImageQuality(Stream stream, byte compression)
        {
            Image image = Image.FromStream(stream);
            //EncoderParameters eps = new EncoderParameters();
            //ImageCodecInfo ici = this.GetEncoderInfo("image/jpeg");

            //eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);

            image.Save(stream, ImageFormat.Jpeg); //, ici, eps);

            return stream;
        }

        public Stream CreateThumbnailProportional(Stream stream, int width, int height)
        {
            int nWidth, nHeight;
            double pct;
            Bitmap bitmap = new Bitmap(stream);
            Image image = Image.FromStream(stream);

            if (bitmap.Size.Width > width)
            {
                pct = (double)(image.Size.Width - width) / (double)image.Size.Width;
                // decrease height proportional
                nHeight = (int)(image.Size.Height * (1 - pct));
                nWidth = width;

                stream = this.CreateThumbnail(stream, nWidth, nHeight);

                bitmap = (Bitmap)Image.FromStream(stream);
            }

            if (bitmap.Size.Height > height)
            {
                pct = (double)(image.Size.Height - height) / (double)image.Size.Height;
                // decrease width proportional
                nHeight = height;
                nWidth = (int)(image.Size.Width * (1 - pct));

                stream = this.CreateThumbnail(stream, nWidth, nHeight);
            }

            return stream;
        }

        public System.IO.Stream CreateThumbnail(System.IO.Stream st, double wid, double hght)
        {
            var ms = new MemoryStream();

            using (Image imagesize = Image.FromStream(st)) // using (Image imagesize = Image.FromFile(f.FullName))
            {
                using (Bitmap bitmapNew = new Bitmap(imagesize))
                {
                    double maxWidth = wid;
                    double maxHeight = hght;
                    int w = imagesize.Width;
                    int h = imagesize.Height;
                    // Longest and shortest dimension 
                    int longestDimension = (w > h) ? w : h;
                    int shortestDimension = (w < h) ? w : h;
                    // propotionality  
                    float factor = ((float)longestDimension) / shortestDimension;
                    // default width is greater than height    
                    double newWidth = maxWidth;
                    double newHeight = maxWidth / factor;
                    // if height greater than width recalculate  
                    if (w < h)
                    {
                        newWidth = maxHeight / factor;
                        newHeight = maxHeight;
                    }

                    bitmapNew.GetThumbnailImage((int)newWidth, (int)newHeight, () => false, IntPtr.Zero)
                        .Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

            return ms;
        }

    }

}
