using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GoogleImageShell
{
    public static class ShortcutMenu
    {
        private const string ShellKeyPathFormat = @"Software\Classes\SystemFileAssociations\{0}\shell";
        private const string VerbName = "GoogleImageShell";
        private const string CommandKey = "command";
        private static readonly Dictionary<ImageFileType, string[]> FileTypeMap = new Dictionary<ImageFileType, string[]>
        {
            {ImageFileType.JPG, new[] {".jpg", ".jpe", ".jpeg", ".jfif"}},
            {ImageFileType.GIF, new[] {".gif"}},
            {ImageFileType.PNG, new[] {".png"}},
            {ImageFileType.BMP, new[] {".bmp"}}
        };

        private static string CreateProgramCommand(bool includeFileName)
        {
            string exePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string command = $"\"{exePath}\" search \"%1\"";
            if (includeFileName)
            {
                command += " -n";
            }
            return command;
        }

        private static RegistryKey GetShellKey(bool allUsers, string fileType)
        {
            RegistryKey hiveKey = allUsers ? Registry.LocalMachine : Registry.CurrentUser;
            string shellPath = string.Format(ShellKeyPathFormat, fileType);
            RegistryKey shellKey = hiveKey.CreateSubKey(shellPath);
            return shellKey;
        }

        public static void InstallHandler(string menuText, bool includeFileName, bool allUsers, ImageFileType[] types)
        {
            string command = CreateProgramCommand(includeFileName);
            foreach (ImageFileType fileType in types)
            {
                foreach (string typeExt in FileTypeMap[fileType])
                {
                    using (RegistryKey shellKey = GetShellKey(allUsers, typeExt))
                    using (RegistryKey verbKey = shellKey.CreateSubKey(VerbName))
                    using (RegistryKey cmdKey = verbKey.CreateSubKey(CommandKey))
                    {
                        verbKey.SetValue("", menuText);
                        cmdKey.SetValue("", command);
                    }
                }
            }
        }

        public static void UninstallHandler(bool allUsers, ImageFileType[] types)
        {
            foreach (ImageFileType fileType in types)
            {
                foreach (string typeExt in FileTypeMap[fileType])
                {
                    using (RegistryKey shellKey = GetShellKey(allUsers, typeExt))
                    {
                        shellKey?.DeleteSubKeyTree(VerbName, false);
                    }
                }
            }
        }
    }
}
