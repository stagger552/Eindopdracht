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
using Menu = Eindopdracht.View.Menu;

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
            Application.Current.MainWindow.WindowState = WindowState.Maximized;



        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the home window

            // Create new window and apply the same position and size
            Home homeWindow = new Home

            {
                WindowState = WindowState.Maximized // Set fullscreen
            };

            // Show the home window
            homeWindow.Show();
            // Optionally, close the current window
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}