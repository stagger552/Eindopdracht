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
using Eindopdracht.View;

namespace Eindopdracht
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Home homeWindow = new Home();
            // Show the home window
            homeWindow.Show();
            //InitializeComponent();

        }

   
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the home window
            Home homeWindow = new Home();
            // Show the home window
            homeWindow.Show();
            // Optionally, close the current window
            this.Close();
        }
    }
}