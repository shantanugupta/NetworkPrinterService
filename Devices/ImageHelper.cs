using ImageMagick;
using System.Drawing;

namespace Devices
{
    public class ImageHelper
    {
        public static byte[] GetPhotoFileContent(string imagePath, uint imageWidth, uint imageHeight)
        {
            byte[] fileContent;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(imagePath).Result;
                if (response.IsSuccessStatusCode)
                {
                    using (Stream stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        fileContent = ResizeAndCropImage(stream, imageWidth, imageHeight);
                    }
                }
                else
                {
                    throw new Exception("Could not load image");
                }
            }

            return fileContent;
        }

        private static byte[] ResizeAndCropImage(Stream imageStream, uint width, uint height)
        {
            using (var image = new MagickImage(imageStream))
            {
                // Resize the image to fill the specified dimensions, maintaining aspect ratio
                image.Resize(width, height);

                // Crop the image to the center
                image.Crop(new MagickGeometry(width, height) { IgnoreAspectRatio = true });

                // Convert to byte array
                using (var ms = new MemoryStream())
                {
                    image.Write(ms, MagickFormat.Png); // You can change format if needed
                    return ms.ToArray();
                }
            }
        }

        public static Image ConvertUrlToSystemDrawingImage(string url, uint imageWidth, uint imageHeight)
        {
            // Download the image as a byte array
            byte[] imageData = GetPhotoFileContent(url, imageWidth, imageHeight);

            // Load the image into MagickImage
            using (MagickImage magickImage = new MagickImage(imageData))
            {
                // Convert the MagickImage to a Bitmap
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    magickImage.Write(memoryStream, MagickFormat.Bmp);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Load the Bitmap from the MemoryStream
                    return new Bitmap(memoryStream);
                }
            }
        }
    }
}
