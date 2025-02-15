﻿using DocMgrLib.Models;
using DocMgrLib.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CWDocMgrApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAccountService _accountService;
        private readonly IDocumentService _documentService;

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

        public MainWindow() { }

        public MainWindow(IAccountService accountService, IDocumentService documentService)
        {
            _accountService = accountService;
            _documentService = documentService;

            InitializeComponent();

            docCollection = new ObservableCollection<DocumentGridVM>
            {
                new DocumentGridVM{
                    DocumentName = "SomeDoc",
                    OriginalDocumentName = "OriginalDoc",
                    UserName = "Fred",
                    DocumentDate = 123456789
                }
            };
            DataContext = this;

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

                var newCollection = _documentService.UploadDocuments(openFileDlg.FileNames, _accountService.loggedInUser);
                docCollection.Clear();
                foreach (DocumentGridVM vm in newCollection)
                {
                    docCollection.Add(vm);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}