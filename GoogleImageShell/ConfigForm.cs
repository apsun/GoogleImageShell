using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace GoogleImageShell
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text += $" v{version.Major}.{version.Minor}.{version.Build}";
            foreach (object type in Enum.GetValues(typeof(ImageFileType)))
            {
                fileTypeListBox.Items.Add(type, true);
            }
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            string menuText = menuTextTextBox.Text;
            bool includeFileName = includeFileNameCheckBox.Checked;
            bool allUsers = allUsersCheckBox.Checked;
            bool resizeOnUpload = resizeOnUploadCheckbox.Checked;
            ImageFileType[] types = fileTypeListBox.CheckedItems.Cast<ImageFileType>().ToArray();
            Install(menuText, includeFileName, allUsers, resizeOnUpload, types);
        }

        private void uninstallButton_Click(object sender, EventArgs e)
        {
            bool allUsers = allUsersCheckBox.Checked;
            ImageFileType[] types = fileTypeListBox.CheckedItems.Cast<ImageFileType>().ToArray();
            Uninstall(allUsers, types);
        }

        private static void Install(string menuText, bool includeFileName, bool allUsers, bool resizeOnUpload, ImageFileType[] types)
        {
            try
            {
                ShortcutMenu.InstallHandler(menuText, includeFileName, allUsers, resizeOnUpload, types);
            }
            catch (Exception ex)
            {
                ErrorMsgBox(
                    "Installation failed",
                    "Could not add context menu entries to Windows Explorer.\n\n" +
                    ex.Message);
                return;
            }
            InfoMsgBox(
                "Installation succeeded",
                "Context menu entries were added to Windows Explorer. " +
                "Remember to reinstall the program if you move or rename it!");
        }

        private static void Uninstall(bool allUsers, ImageFileType[] types)
        {
            try
            {
                ShortcutMenu.UninstallHandler(allUsers, types);
            }
            catch (Exception ex)
            {
                ErrorMsgBox(
                    "Uninstallation failed",
                    "Could not remove context menu entries from Windows Explorer.\n\n" + 
                    ex.Message);
                return;
            }
            InfoMsgBox(
                "Uninstallation succeeded",
                "Context menu entries were removed from Windows Explorer.");
        }

        private static void InfoMsgBox(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ErrorMsgBox(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
