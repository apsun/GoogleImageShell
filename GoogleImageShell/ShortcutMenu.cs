using Microsoft.Win32;
using System;
using System.Reflection;

namespace GoogleImageShell
{
    public static class ShortcutMenu
    {
        private const string ShellKeyPathFormat = @"Software\Classes\SystemFileAssociations\{0}\shell";
        private const string VerbName = "GoogleImageShell";
        private const string VerbTitle = "Search on Google Images";
        private const string CommandKey = "command";
        private static readonly string[] FileTypes = {
            ".jpg", ".jpe", ".jpeg", ".jfif",
            ".gif", ".png", ".bmp",
        };

        private static string GetProgramCommandTemplate()
        {
            string exePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return string.Format("\"{0}\" search \"%1\"", exePath);
        }

        private static RegistryKey GetShellKey(bool allUsers, string fileType)
        {
            RegistryKey hiveKey = allUsers ? Registry.LocalMachine : Registry.CurrentUser;
            string shellPath = string.Format(ShellKeyPathFormat, fileType);
            RegistryKey shellKey = hiveKey.CreateSubKey(shellPath);
            return shellKey;
        }

        public static void InstallHandler(bool allUsers)
        {
            string cmdTemplate = GetProgramCommandTemplate();
            foreach (string fileType in FileTypes)
            {
                using (RegistryKey shellKey = GetShellKey(allUsers, fileType))
                using (RegistryKey verbKey = shellKey.CreateSubKey(VerbName))
                using (RegistryKey cmdKey = verbKey.CreateSubKey(CommandKey))
                {
                    verbKey.SetValue("", VerbTitle);
                    cmdKey.SetValue("", cmdTemplate);
                }
            }
        }

        public static void UninstallHandler(bool allUsers)
        {
            foreach (string fileType in FileTypes)
            {
                using (RegistryKey shellKey = GetShellKey(allUsers, fileType))
                {
                    if (shellKey != null)
                    {
                        shellKey.DeleteSubKeyTree(VerbName, false);
                    }
                }
            }
        }
    }
}
