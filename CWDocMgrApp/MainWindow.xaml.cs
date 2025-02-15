using DocMgrLib.Services;
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

        public MainWindow() { }

        public MainWindow(IAccountService accountService)
        {
            _accountService = accountService;

            InitializeComponent();

            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                // Handle successful login
                string username = loginWindow.Username;
                string password = loginWindow.Password;

                _accountService.Login(username, password);
            }
            else
            {
                // Handle login cancellation
                Application.Current.Shutdown();
            }
        }
    }
}