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

namespace Eindopdracht
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

   
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtGebruikernaam.Text;
            string password = txtPassword.Text;

            
            Console.WriteLine(username + ":" + password);
            Console.WriteLine(password);
        }
    }
}