using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleImageShell
{
    public partial class UploadForm : Form
    {
        private readonly string _imagePath;
        private readonly bool _includeFileName;
        private readonly bool _resizeOnUpload;

        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public UploadForm(string[] args)
        {
            InitializeComponent();
            for (int i = 1; i < args.Length; ++i)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-n":
                        _includeFileName = true;
                        break;
                    case "-r":
                        _resizeOnUpload = true;
                        break;
                    default:
                        _imagePath = arg;
                        break;
                }
            }
        }

        private void UploadForm_Load(object sender, EventArgs e)
        {
            Log("Uploading image: " + _imagePath);
            Log("Include file name: " + _includeFileName);
            Log("Resize on upload: " + _resizeOnUpload);

            Task<string> task = GoogleImages.Search(_imagePath, _includeFileName, _resizeOnUpload, _cancelTokenSource.Token);
            task.ContinueWith(OnUploadComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UploadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancelTokenSource.Cancel();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnUploadComplete(Task<string> task)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                Log("Failed to upload image: " + task.Exception.InnerException);
            }
            else if (task.Status == TaskStatus.Canceled)
            {
                Log("Upload canceled by user");
            }
            else if (task.Status == TaskStatus.RanToCompletion)
            {
                Log("Image uploaded successfully, opening results page");
                if (TryOpenBrowser(task))
                {
                    Close();
                    return;
                }
            }
            else
            {
                Log("Unexpected task result status: " + task.Status);
            }
            cancelButton.Text = "Close";
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
        }

        private bool TryOpenBrowser(Task<string> task)
        {
            try
            {
                Process.Start(task.Result);
                return true;
            }
            catch (Exception ex)
            {
                Log("Failed to open browser: " + ex);
                return false;
            }
        }

        private void Log(string text)
        {
            logTextBox.AppendText(text);
            logTextBox.AppendText(Environment.NewLine);
        }
    }
}
