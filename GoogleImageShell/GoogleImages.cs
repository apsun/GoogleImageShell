using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleImageShell
{
    public static class GoogleImages
    {
        private const int MinImageDimension = 200;
        private const int MaxImageDimension = 800;

        /// <summary>
        /// Determines whether the input image should be resized,
        /// and if so, the optimal dimensions after resizing.
        /// </summary>
        /// <param name="originalSize">Original size of the image</param>
        /// <param name="newSize">Dimensions after resizing</param>
        /// <returns>true if the image should be resized; false otherwise</returns>
        private static bool ShouldResize(Size originalSize, out Size newSize)
        {
            // Compute resize ratio (at LEAST ratioMin, at MOST ratioMax).
            // ratioMin is used to prevent the image from getting too small.
            // Note that ratioMax is calculated on the LARGER image dimension,
            // whereas ratioMin is calculated on the SMALLER image dimension.
            int origW = originalSize.Width;
            int origH = originalSize.Height;
            double ratioMax = Math.Min(MaxImageDimension / (double)origW, MaxImageDimension / (double)origH);
            double ratioMin = Math.Max(MinImageDimension / (double)origW, MinImageDimension / (double)origH);
            double ratio = Math.Max(ratioMax, ratioMin);

            // If resizing it would make it bigger, then don't bother
            if (ratio >= 1)
            {
                newSize = originalSize;
                return false;
            }

            int newW = (int)(origW * ratio);
            int newH = (int)(origH * ratio);
            newSize = new Size(newW, newH);
            return true;
        }

        /// <summary>
        /// Loads an image from disk into a byte array.
        /// </summary>
        /// <param name="imagePath">Path to the image file</param>
        /// <param name="resize">Whether to allow resizing</param>
        /// <returns>The loaded image, represented as a byte array</returns>
        private static byte[] LoadImageData(string imagePath, bool resize)
        {
            // Resize the image if user enabled the option
            // and the image is reasonably large
            if (resize)
            {
                try
                {
                    using (Bitmap bmp = new Bitmap(imagePath))
                    {
                        if (ShouldResize(bmp.Size, out Size newSize))
                        {
                            using (var newBmp = new Bitmap(newSize.Width, newSize.Height))
                            {
                                using (Graphics g = Graphics.FromImage(newBmp))
                                {
                                    g.DrawImage(bmp, new Rectangle(0, 0, newSize.Width, newSize.Height));
                                }

                                // Save as JPEG (format doesn't have to match file extension,
                                // Google will take care of figuring out the correct format)
                                using (var ms = new MemoryStream())
                                {
                                    newBmp.Save(ms, ImageFormat.Jpeg);
                                    return ms.ToArray();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore exceptions (out of memory, invalid format, etc)
                    // and fall back to just reading the raw file bytes
                }
            }

            // No resizing required or image is too small,
            // just load the bytes from disk directly
            return File.ReadAllBytes(imagePath);
        }

        /// <summary>
        /// Converts a byte array into base-64 format, using
        /// a format compatible with Google Images.
        /// </summary>
        /// <param name="content">Raw bytes to encode</param>
        /// <returns>Base-64 encoded string</returns>
        private static string BinaryToBase64Compat(byte[] content)
        {
            // Uploaded image needs to be encoded in base-64,
            // with `+` replaced by `-` and `/` replaced by `_`
            string base64 = Convert.ToBase64String(content).Replace('+', '-').Replace('/', '_');
            return base64;
        }

        /// <summary>
        /// Asynchronously uploads the specified image to Google Images,
        /// and returns the URL of the results page.
        /// </summary>
        /// <param name="imagePath">Path to the image file</param>
        /// <param name="includeFileName">Whether to send the image file name to Google</param>
        /// <param name="resizeOnUpload">Whether to resize large images</param>
        /// <param name="cancelToken">Allows for cancellation of the upload</param>
        /// <returns>String containing the URL of the results page</returns>
        public static async Task<string> Search(string imagePath, bool includeFileName, bool resizeOnUpload, CancellationToken cancelToken)
        {
            // Load the image, resizing it if necessary
            byte[] data = LoadImageData(imagePath, resizeOnUpload);

            // Prevent auto redirect (we want to open the
            // redirect destination directly in the browser)
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;

            using (var client = new HttpClient(handler))
            {
                var form = new MultipartFormDataContentCompat();
                form.Add(new StringContent(BinaryToBase64Compat(data)), "image_content");
                if (includeFileName)
                {
                    form.Add(new StringContent(Path.GetFileName(imagePath)), "filename");
                }
                var response = await client.PostAsync("https://images.google.com/searchbyimage/upload", form, cancelToken);
                if (response.StatusCode != HttpStatusCode.Redirect)
                {
                    throw new IOException("Expected redirect to results page, got " + (int)response.StatusCode);
                }
                string resultUrl = response.Headers.Location.ToString();
                return resultUrl;
            }
        }

        /// <summary>
        /// Google Images has some oddities in the way it requires
        /// forms data to be uploaded. The main three that I could
        /// find are:
        ///
        /// 1. Content-Disposition name parameters must be quoted
        /// 2. Content-Type boundary parameter must NOT be quoted
        /// 3. Image base-64 encoding replaces `+` -> `-`, `/` -> `_`
        ///
        /// This class transparently handles the first two quirks.
        /// </summary>
        private class MultipartFormDataContentCompat : MultipartContent
        {
            public MultipartFormDataContentCompat() : base("form-data")
            {
                FixBoundaryParameter();
            }

            public MultipartFormDataContentCompat(string boundary) : base("form-data", boundary)
            {
                FixBoundaryParameter();
            }

            public override void Add(HttpContent content)
            {
                base.Add(content);
                AddContentDisposition(content, null, null);
            }

            public void Add(HttpContent content, string name)
            {
                base.Add(content);
                AddContentDisposition(content, name, null);
            }

            public void Add(HttpContent content, string name, string fileName)
            {
                base.Add(content);
                AddContentDisposition(content, name, fileName);
            }

            private void AddContentDisposition(HttpContent content, string name, string fileName)
            {
                var headers = content.Headers;
                if (headers.ContentDisposition == null)
                {
                    headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = QuoteString(name),
                        FileName = QuoteString(fileName)
                    };
                }
            }

            private void FixBoundaryParameter()
            {
                var boundary = Headers.ContentType.Parameters.Single(p => p.Name == "boundary");
                boundary.Value = boundary.Value.Trim('"');
            }

            private static string QuoteString(string str)
            {
                return '"' + str + '"';
            }
        }
    }
}
