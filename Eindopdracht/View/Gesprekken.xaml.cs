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
using System.IO;

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

        private bool geluidChecked = true;
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
            Home home = new Home() 
            {
                WindowState = WindowState.Maximized // Set fullscreen
            };
            home.Show();

            this.Close();

        }

        private void Open_Gesprekken(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Gesprekken gesprekken = new Gesprekken();

            // Set the new window's position and size to match the current window
            gesprekken.WindowStartupLocation = WindowStartupLocation.Manual;
            gesprekken.Left = this.Left;
            gesprekken.Top = this.Top;
            gesprekken.Width = this.ActualWidth;
            gesprekken.Height = this.ActualHeight;


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

            Database database = new Database();

            // Set the new window's position and size to match the current window
            database.WindowStartupLocation = WindowStartupLocation.Manual;
            database.Left = this.Left;
            database.Top = this.Top;
            database.Width = this.ActualWidth;
            database.Height = this.ActualHeight;

            database.Show();

            this.Close();
        }

        private void btnOphangen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inloggen = new MainWindow();

            // Set the new window's position and size to match the current window
            inloggen.WindowStartupLocation = WindowStartupLocation.Manual;
            inloggen.Left = this.Left;
            inloggen.Top = this.Top;
            inloggen.Width = this.ActualWidth;
            inloggen.Height = this.ActualHeight;


            this.Close();
            inloggen.Show();

        }
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateActiveCalls();
            update24hourcalls();
            LoadData();
            lastReceived();

        }
        private void PlaySound(string soundFile)
        {
            try
            {
                // Create a new SoundPlayer object and set the sound file path
                var player = new System.Media.SoundPlayer(soundFile);

                // Play the sound synchronously
                player.Play();
            }
            catch (Exception ex)
            {
                // Handle any errors, such as file not found or format issues

                Console.WriteLine(ex.Message);
            }
        }

        public async void updateActiveCalls()
        {
            try
            {




                int? NewCallerAmount = await gesprekkenController.GetActiveCalls();


                if (NewCallerAmount >= 0)
                {
                    int PreviousCallerAmount = 0; // Initialize with a default value

                    // Try to get the previous caller amount from the UI
                    try
                    {
                        lblLivecalls.Dispatcher.Invoke(() =>
                        {
                            if (lblLivecalls.Content != "")
                            {
                                PreviousCallerAmount = Convert.ToInt32(lblLivecalls.Content);

                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        // Optionally handle the error (for example, set PreviousCallerAmount to 0 if conversion fails)
                        PreviousCallerAmount = 0;
                    }

                    // Update UI with Green color when calls are active
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1DD75B"));
                    });

                    // Update the label with the new number of active calls
                    lblLivecalls.Dispatcher.Invoke(() => { lblLivecalls.Content = NewCallerAmount.ToString(); });

                    if (geluidChecked)
                    {
                        string path;
                        // Play sound based on the number of calls change
                        if (NewCallerAmount > PreviousCallerAmount)
                        {
                             path = System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "..", "..", "..", "View", "source", "Sound", "NewCall.wav"
                            );

                            PlaySound(path);
                        }
                        else if (NewCallerAmount < PreviousCallerAmount)
                        {
                            path = System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                "..", "..", "..", "View", "source", "Sound", "EndCall.wav"
                            );
                            PlaySound(path);
                        }
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE3B40"));
                    });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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

        public async void lastReceived()
        {

            var LastReceived = gesprekkenController.GetDatetime();

            // Use the Dispatcher to update the UI on the main thread
            lblLastReceived.Dispatcher.Invoke(() =>
            {
                lblLastReceived.Content = LastReceived.Result.ToString();
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

            try
            {

                if (sender is Button button && button.Tag is CallData callData)
                {
                    GesprekDetails gesprekDetails = new GesprekDetails(callData.CallInfo, callData.IsClosed)

                    {
                        WindowState = WindowState.Maximized
                    };

                    this.Close();

                    gesprekDetails.Show();


                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
                throw;
            }
            
         
        }

        private void cbxGeluid_Checked(object sender, RoutedEventArgs e)
        {
            geluidChecked = true; // Update the variable when checked
            Console.WriteLine($"CheckBox is checked: {geluidChecked}");
        }

        private void cbxGeluid_Unchecked(object sender, RoutedEventArgs e)
        {
            geluidChecked = false; // Update the variable when unchecked
            Console.WriteLine($"CheckBox is checked: {geluidChecked}");
        }
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


        [JsonProperty("PlannedDate")]
        public bool PlannedDate { get; set; }

        [JsonProperty("Datetime")]
        public string Datetime { get; set; }
    }

}
