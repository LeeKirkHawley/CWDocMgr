using DocMgrLib.Models;
using DocMgrLib.Services;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CWDocMgrApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountService _accountService;
        private readonly IDocumentService _documentService;

        public MainWindow() { }

        public MainWindow(IAccountService accountService, IDocumentService documentService)
        {
            _accountService = accountService;
            _documentService = documentService;

            InitializeComponent();

            //var loginWindow = new LoginWindow();
            //if (loginWindow.ShowDialog() == true)
            //{
            //    // Handle successful login
            //    string username = loginWindow.Username;
            //    string password = loginWindow.Password;

            //    _accountService.Login(username, password);
            //}
            //else
            //{
            //    // Handle login cancellation
            //    Application.Current.Shutdown();
            //}
            _accountService.Login("Kirk", "pwd");
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Multiselect = true;

            // Set filter for file extension and default file extension 
            //openFileDlg.DefaultExt = ".txt";
            //openFileDlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                string filename = openFileDlg.FileName;
                //FilePathText.Text = $"Selected file: {filename}";

                UploadDocsViewModel uploadDpcsVM = new UploadDocsViewModel
                {
                    OriginalFileName = filename
                };
                _documentService.UploadDocuments(openFileDlg.FileNames, _accountService.loggedInUser);
            }
        }
    }
}