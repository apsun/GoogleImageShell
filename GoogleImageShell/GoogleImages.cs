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
        private const int _maxImageSize = 800;

        public static async Task<string> Search(string imagePath, bool includeFileName, bool resizeOnUpload, CancellationToken cancelToken)
        {
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;

            StringContent content = null;


            if (resizeOnUpload)
            {
                try
                {

                }
                catch (Exception)
                {

                }
                using (Bitmap bmp = new Bitmap(imagePath))
                {
                    if (bmp.Width > _maxImageSize || bmp.Height > _maxImageSize)
                    {
                        var newSize = ResizeKeepAspect(bmp.Size, _maxImageSize, _maxImageSize);

                        using (var newBmp = new Bitmap(newSize.Width, newSize.Height))
                        {
                            using (Graphics g = Graphics.FromImage(newBmp))
                            {
                                g.DrawImage(bmp, new Rectangle(0, 0, newSize.Width, newSize.Height));
                            }

                            using (var ms = new MemoryStream())
                            {
                                newBmp.Save(ms, ImageFormat.Jpeg);

                                content = new StringContent(BinaryToBase64Compat(ms.ToArray()));
                            }
                        }
                    }
                }
            }



            if (content == null)
            {
                content = new StringContent(FileToBase64Compat(imagePath));
            }

            using (var client = new HttpClient(handler))
            {
                var form = new MultipartFormDataContentCompat();
                form.Add(content, "image_content");
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

        private static string BinaryToBase64Compat(byte[] content)
        {
            string base64 = Convert.ToBase64String(content).Replace('+', '-').Replace('/', '_');
            return base64;
        }


        private static string FileToBase64Compat(string imagePath)
        {
            byte[] content = File.ReadAllBytes(imagePath);
            string base64 = BinaryToBase64Compat(content);
            return base64;
        }

        public static Size ResizeKeepAspect(Size src, int maxWidth, int maxHeight)
        {
            decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
            return new Size((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
        }

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
