using AutoMapper;
using DocMgrLib.Models;
using DocMgrLib.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CWDocMgrApp
{
    public partial class MainWindow : Window
    {
        private readonly IAccountService _accountService;
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IOCRService _ocrService;
        private readonly IMapper _mapper;

        public ObservableCollection<DocumentGridVM> _docCollection = [];
        public ObservableCollection<DocumentGridVM> docCollection
        {
            get { return _docCollection; }
            set
            {
                _docCollection = value;
                OnPropertyChanged();
            }
        }

        private DocumentGridVM _selectedDocument;
        public DocumentGridVM SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                OnPropertyChanged();
                if (_selectedDocument == null)
                {
                }
                else
                {
                    ViewDocumentDetails(_selectedDocument);
                }
            }
        }
        public ICommand OcrCommand { get; }
        public ICommand DeleteCommand { get; }


        public MainWindow() { }

        public MainWindow(IAccountService accountService, IDocumentService documentService,
            IUserService userService, IFileService fileService, IOCRService ocrService, IMapper mapper)
        {
            _accountService = accountService;
            _documentService = documentService;
            _userService = userService;
            _fileService = fileService;
            _mapper = mapper;
            _ocrService = ocrService;

            InitializeComponent();

            DataContext = this;

            // PERMANENT
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

            // TEMPORARY 

            OcrCommand = new RelayCommand<DocumentGridVM>(OcrDocument);
            DeleteCommand = new RelayCommand<DocumentGridVM>(DeleteDocument);

            _accountService.Login("Kirk", "pwd");

            LoadFromDatabase();
        }

        public void LoadFromDatabase()
        {
            ClaimsPrincipal? loggedInUser = _accountService.loggedInUser;
            if (loggedInUser == null || loggedInUser.Identity?.Name == null)
            {
                return;
            }
            ClaimsIdentity identity = loggedInUser.Identities.ToArray()[0];
            UserModel user = _userService.GetAllowedUser(loggedInUser.Identity.Name);

            IEnumerable<DocumentModel> docsFromDB = _documentService.GetDocuments(user, 1, 10);

            List<DocumentGridVM> vms = new List<DocumentGridVM>();
            foreach (DocumentModel doc in docsFromDB)
            {
                DocumentGridVM vm = new DocumentGridVM
                {
                    Id = doc.Id,
                    DocumentName = doc.DocumentName,
                    OriginalDocumentName = doc.OriginalDocumentName,
                    UserName = user.userName,
                    DocumentDate = doc.DocumentDate
                };

                vms.Add(vm);
            }

            docCollection.Clear();
            foreach (DocumentGridVM vm in vms)
            {
                docCollection.Add(vm);
            }

            OnPropertyChanged();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Multiselect = true;

            // Set filter for file extension and default file extension 
            openFileDlg.DefaultExt = ".jpg;*.png;*.pdf";
            openFileDlg.Filter = "Image and PDF Files|*.jpg;*.jpeg;*.png;*.pdf|JPEG Files|*.jpg;*.jpeg|PNG Files|*.png|PDF Files|*.pdf";

            bool? result = openFileDlg.ShowDialog();

            if (result == true)
            {
                string filename = openFileDlg.FileName;

                UploadDocsViewModel uploadDpcsVM = new UploadDocsViewModel
                {
                    OriginalFileName = filename
                };

                var newCollection = _documentService.UploadDocuments(openFileDlg.FileNames, _accountService.loggedInUser);

                LoadFromDatabase();
            }
        }

        private void OcrDocument(DocumentGridVM document)
        {
            DocumentModel docModel = new DocumentModel
            {
                Id = document.Id,
                DocumentName = document.DocumentName,
                OriginalDocumentName = document.OriginalDocumentName,
                DocumentDate = document.DocumentDate,
                UserId = _userService.GetAllowedUser(document.UserName).Id
            };

            _ocrService.DoOcr(docModel);

            SelectedDocument = document;
            ViewDocumentDetails(document);
        }

        private void ViewDocumentDetails(DocumentGridVM document)
        {
            if (document.DocumentName.Contains(".pdf"))
            {
                // haven't implemented PDF display yet
                DisplayedImage.Source = null;
            }
            else
            {
                string filePath = _fileService.GetDocFilePath(document.DocumentName);

                try
                {
                    ReadDocumentFile(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }

            string ocrTextFile = _fileService.GetOcrFilePath(document.DocumentName);
            if (File.Exists(ocrTextFile))
            {
                string ocrText = File.ReadAllText(ocrTextFile);
                DisplayedOcr.Text = ocrText;
            }
            else
            {
                DisplayedOcr.Text = "";
            }
        }

        private void ReadDocumentFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
                DisplayedImage.Source = image;
            }
        }

        private void DeleteDocument(DocumentGridVM document)
        {
            _documentService.DeleteDocument(document.Id);
            DisplayedOcr.Text = "";
            LoadFromDatabase();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;

            public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute((T)parameter);
            }

            public void Execute(object parameter)
            {
                _execute((T)parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

        }
    }
}