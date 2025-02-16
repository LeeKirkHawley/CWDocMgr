using AutoMapper;
using DocMgrLib.Models;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Components.Forms;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            }
        }
        public ICommand OcrCommand { get; }
        public ICommand DetailsCommand { get; }
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

            docCollection = new ObservableCollection<DocumentGridVM>
            {
                new DocumentGridVM{
                    Id = 1,
                    DocumentName = "SomeDoc",
                    OriginalDocumentName = "OriginalDoc",
                    UserName = "Fred",
                    DocumentDate = 123456789
                }
            };
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
            DetailsCommand = new RelayCommand<DocumentGridVM>(ViewDocumentDetails);
            DeleteCommand = new RelayCommand<DocumentGridVM>(DeleteDocument);

            _accountService.Login("Kirk", "pwd");

            LoadFromDatabase();
        }

        private void LoadFromDatabase()
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

                var newCollection = _documentService.UploadDocuments(openFileDlg.FileNames, _accountService.loggedInUser);

                LoadFromDatabase();
            }
        }

        private void OcrDocument(DocumentGridVM document)
        {
            DocumentModel docModel = new DocumentModel {
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
                DisplayedImage.Source = null;
            }
            else
            {
                string filePath = _fileService.GetDocFilePath(document.DocumentName);
                BitmapImage image = new BitmapImage(new Uri(filePath));
                DisplayedImage.Source = image;
            }

            string ocrTextFile = _fileService.GetOcrFilePath(document.DocumentName);
            if(File.Exists(ocrTextFile))
            {
                string ocrText = File.ReadAllText(ocrTextFile);
                DisplayedOcr.Text = ocrText;
            }
        }

        private void DeleteDocument(DocumentGridVM document)
        {
            _documentService.DeleteDocument(document.Id);
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