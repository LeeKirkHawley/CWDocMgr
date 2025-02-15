using System.Windows;
using System.Windows.Controls;

namespace CWDocMgrApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameTextBox.Text;
            Password = PasswordBox.Password;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
