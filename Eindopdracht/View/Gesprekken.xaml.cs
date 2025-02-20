using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Eindopdracht.Controller;
using System.Timers;
namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for Gesprekken.xaml
    /// </summary>
    public partial class Gesprekken : Window
    {
        private System.Timers.Timer timer;
        private GesprekkenController gesprekkenController = new GesprekkenController();

        public Gesprekken()
        {
            InitializeComponent();

            timer = new System.Timers.Timer(1000); // 5000 milliseconds = 5 seconds
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        private void Open_Home(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Home home = new Home();

            home.Show();

            this.Close();

        }

        private void Open_Gesprekken(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Gesprekken gesprekken = new Gesprekken();

            gesprekken.Show();

            this.Close();
        }

        private void Open_Account(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Account account = new Account();
            account.Show();

            this.Close();
        }

        private void Open_Database(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Database database = new Database();
            database.Show();

            this.Close();
        }

        private void btnOphangen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inloggen = new MainWindow();
            this.Close();
            inloggen.Show();

        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateActiveCalls();
            update24hourcalls();
            GetCallData();
        }

        public async void updateActiveCalls()
        {

            var aantal = gesprekkenController.GetActiveCalls();

            // Use the Dispatcher to update the UI on the main thread
            lblLivecalls.Dispatcher.Invoke(() =>
            {
                lblLivecalls.Content = aantal.Result.ToString();
            });
        }

        public async void update24hourcalls()
        {

            var aantal = gesprekkenController.Get24Callls();

            // Use the Dispatcher to update the UI on the main thread
            lblGesprekVandaag.Dispatcher.Invoke(() =>
            {
                lblGesprekVandaag.Content = aantal.Result.ToString();
            });
        }

        public async void GetCallData()
        {

            var calldata = gesprekkenController.GetJSON();

            

            // Use the Dispatcher to update the UI on the main thread
            txtCalldata.Dispatcher.Invoke(() =>
            {
                txtCalldata.Text= calldata.Result.ToString();
            });
        }

    }
}
