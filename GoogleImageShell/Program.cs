using System;
using System.Diagnostics;

namespace GoogleImageShell
{
    public static class Program
    {
        private static void PrintUsage()
        {
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            Console.Error.WriteLine(
                "usage: " + exeName + " <args>\n" +
                "args:\n" +
                "  install [-a]    - Add this program to the Windows Explorer context menu\n" +
                "    -a            - Add for all users (requires administrator privileges)\n" +
                "  uninstall [-a]  - Remove this program from the Windows Explorer context menu\n" +
                "    -a            - Remove for all users (requires administrator privileges)\n" +
                "  search <path>   - Uploads the specified image to Google image search\n" +
                "    path          - Path to the image to upload");
        }

        private static int Search(string imagePath)
        {
            Console.WriteLine("Uploading image to Google image search...");
            string resultUrl;
            try
            {
                resultUrl = GoogleImages.Search(imagePath).Result;
            }
            catch (AggregateException ex)
            {
                Console.Error.WriteLine("Error: Failed to upload image");
                Console.Error.WriteLine(ex.InnerException);
                return 1;
            }
            Console.WriteLine("Opening search results in browser...");
            try
            {
                Process.Start(resultUrl);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: Failed to open browser");
                Console.Error.WriteLine(ex);
                return 1;
            }
            Console.WriteLine("Done!");
            return 0;
        }

        private static int Install(bool allUsers)
        {
            Console.WriteLine("Adding context menu entries...");
            try
            {
                ShortcutMenu.InstallHandler(allUsers);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: Failed to add context menu entries");
                Console.Error.WriteLine(ex);
                return 1;
            }
            Console.WriteLine("Done!");
            return 0;
        }

        private static int Uninstall(bool allUsers)
        {
            Console.WriteLine("Removing context menu entries...");
            try
            {
                ShortcutMenu.UninstallHandler(allUsers);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: Failed to remove context menu entries");
                Console.Error.WriteLine(ex);
                return 1;
            }
            Console.WriteLine("Done!");
            return 0;
        }

        public static int Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "install") return Install(false);
                if (args[0] == "uninstall") return Uninstall(false);
            }
            else if (args.Length == 2)
            {
                if (args[0] == "search") return Search(args[1]);
                if (args[0] == "install" && args[1] == "-a") return Install(true);
                if (args[0] == "uninstall" && args[1] == "-a") return Uninstall(true);
            }
            PrintUsage();
            return 1;
        }
    }
}
