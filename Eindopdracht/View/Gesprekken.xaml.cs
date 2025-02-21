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
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for Gesprekken.xaml
    /// </summary>
    public partial class Gesprekken : Window
    {
        private System.Timers.Timer timer;
        private GesprekkenController gesprekkenController = new GesprekkenController();
        public ObservableCollection<CallData> Calls { get; set; }

        public Gesprekken()
        {
            InitializeComponent();

            timer = new System.Timers.Timer(1000); // 5000 milliseconds = 5 seconds
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;


            Calls = new ObservableCollection<CallData>();

            this.DataContext = this;

            // Simuleer JSON-data laden
        


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
            LoadData();
        }

        public async void updateActiveCalls()
        {

            int? aantal = await gesprekkenController.GetActiveCalls();



            if(aantal.HasValue)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1DD75B"));
                });


                lblLivecalls.Dispatcher.Invoke(() =>
                {
                    lblLivecalls.Content = aantal.ToString();
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE3B40"));
                });

            }
            // Use the Dispatcher to update the UI on the main thread

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



 

        async void LoadData()
        {
            try
            {
                // Haal JSON op via gesprekkenController
                var json = await gesprekkenController.GetJSON();

                if (json != "Niks")

                {
                    // JSON deserialiseren naar een RootObject
                    var parsedData = JObject.Parse(json);

                    // Check of er data is

                    // Controleer of het verwachte pad aanwezig is in het JSON-object
                    if (parsedData["callId"] is JObject callIdObj)
                    {
                        // UI-bewerkingen via Dispatcher uitvoeren
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Calls.Clear(); // Leeg de lijst in de UI-thread
                            foreach (var call in callIdObj.Properties())
                            {
                                var callData = call.Value.ToObject<CallData>(); // Converteer JObject naar CallData
                                if (callData != null)
                                {
                                    Calls.Add(callData); // Voeg toe in de UI-thread
                                }
                            }
                        });
                    }
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij laden van data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string callId)
            {
                GesprekDetails gesprekDetails = new GesprekDetails(callId);


                this.Close();

                gesprekDetails.Show();


            }
        }

       
    }


    // JSON Parsing Classes
    public class RootObject
    {
        public string Type { get; set; }
        public CallDataWrapper Data { get; set; }
    }

    public class CallDataWrapper
    {
        public Dictionary<string, CallData> CallId { get; set; }
    }

    public class CallData
    {
        [JsonProperty("callinfo")]
        public string CallInfo { get; set; }

        [JsonProperty("Closed")]
        public bool IsClosed { get; set; }

        [JsonProperty("Datetime")]
        public string Datetime { get; set; }
    }

}
