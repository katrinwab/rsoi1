using System.Windows;

namespace labRSOI
{
    /// <summary>
    /// Логика взаимодействия для LogPas.xaml
    /// </summary>
    public partial class LogPas : Window
    {
        public LogPas()
        {
            InitializeComponent();
        }

        public string Username;
        public string Password;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Username = TextBox.Text;
            Password = PasswordBox.Password;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Username = "";
            Password = "";
            Close();
        }
    }
}
