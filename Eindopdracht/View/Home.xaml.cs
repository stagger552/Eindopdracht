using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();



        }
        private void Open_Home(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Home home = new Home()

            // Set the new window's position and size to match the current window

            {
                WindowState = WindowState.Maximized // Set fullscreen
            };


            home.Show();

            this.Close();

        }

        private void Open_Gesprekken(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Gesprekken gesprekken = new Gesprekken()

            {
                WindowState = WindowState.Maximized // Set fullscreen
            };

            gesprekken.Show();

            this.Close();
        }

        private void Open_Account(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Account account = new Account()


            {
                WindowState = WindowState.Maximized // Set fullscreen
            };

            account.Show();

            this.Close();
        }

        private void Open_Database(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Database database = new Database()


            {
                WindowState = WindowState.Maximized // Set fullscreen
            };

            database.Show();

            this.Close();
        }

        private void btnOphangen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inloggen = new MainWindow()


            {
                WindowState = WindowState.Maximized // Set fullscreen
            };



            this.Close();
            inloggen.Show();

        }   
    }
}
