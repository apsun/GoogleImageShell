using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GoogleImageShell
{
    public static class GoogleImages
    {
        public static async Task<string> Search(string imagePath)
        {
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            using (var client = new HttpClient(handler))
            {
                var form = new MultipartFormDataContentCompat();
                form.Add(new StringContent(FileToBase64Compat(imagePath)), "image_content");
                form.Add(new StringContent(Path.GetFileName(imagePath)), "filename");
                var response = await client.PostAsync("https://images.google.com/searchbyimage/upload", form);
                if (response.StatusCode != HttpStatusCode.Redirect)
                {
                    throw new IOException("Expected redirect to results page, got " + (int)response.StatusCode);
                }
                string resultUrl = response.Headers.Location.ToString();
                return resultUrl;
            }
        }

        private static string FileToBase64Compat(string imagePath)
        {
            byte[] content = File.ReadAllBytes(imagePath);
            string base64 = Convert.ToBase64String(content).Replace('+', '-').Replace('/', '_');
            return base64;
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
