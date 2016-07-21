using System;
using System.Windows.Forms;

namespace GoogleImageShell
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length >= 1 && args[0].ToLower() == "search")
            {
                Application.Run(new UploadForm(args));
            }
            else
            {
                Application.Run(new ConfigForm());
            }
        }
    }
}
