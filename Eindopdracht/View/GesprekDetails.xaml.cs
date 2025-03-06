using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Microsoft.VisualBasic;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for GesprekDetails.xaml
    /// </summary>
    public partial class GesprekDetails : Window
    {

        private static readonly HttpClient client = new HttpClient();

        private WebsocketController websocketController = new WebsocketController();


        private System.Timers.Timer timer;

        private string CallerID;
        private bool ConversationClosed;
        private bool dataLoaded;
        public bool FirstError;

        public GesprekDetails(String Callid, bool isClosed)
       {
           CallerID = Callid;
           ConversationClosed = isClosed;
           dataLoaded = false;
           FirstError = false;

            InitializeComponent();

            lblCallID.Content = CallerID;

            
                timer = new System.Timers.Timer(1000); 
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;

          


            // Add converter for button visibility
            ResourceDictionary resources = new ResourceDictionary();
            resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            this.Resources.MergedDictionaries.Add(resources);


            if (isClosed)
            {
                // Subscribe to the Loaded event
                this.Loaded += RemoveItems;

            }
            else
            {
                txtLuisteren.Text = "Live Audio";
            }


        }


        private void RemoveItems(object sender, RoutedEventArgs e)
        { // Hiding the elements
            btnOphangen.Visibility = Visibility.Collapsed;
            btnOpnemen.Visibility = Visibility.Collapsed;
            txtExternalBericht.Visibility = Visibility.Collapsed;
            btnVersturen.Visibility = Visibility.Collapsed;

            txtLuisteren.Text = "Record URL";

        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!FirstError)
                {
                    LoadTranscript();
                    LoadData();
                    lastReceived();
                }
            
            }
            catch (Exception ex)
            {
                if (!FirstError)
                {
                    MessageBox.Show(ex.Message);

                }
            }
        }
        async private void LoadTranscript()
        {
            // Assuming the FullTranscript is in the JSON format
            var json = await websocketController.GetJSON();
            if (json != "Niks")
            {

                var parsedData = JObject.Parse(json);
                var fullTranscript = parsedData["callId"]?[CallerID]?["Transcript"]?["FullTranscript"];


                if (fullTranscript != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var document = new FlowDocument();
                        var paragraph = new Paragraph();
                        foreach (var entry in fullTranscript)
                        {
                            var boldRole = new Bold(new Run($"{entry["role"]}: "));
                            var content = new Run($"{entry["content"]}\n");
                            paragraph.Inlines.Add(boldRole);
                            paragraph.Inlines.Add(content);
                        }
                        document.Blocks.Add(paragraph);
                        TranscriptTextBox.Document = document;
                    });
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1DD75B"));
                });

                // Set the text of the TextBox
            }
            else
            {
                FirstError = true;

                LoadTranscript();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RctStatus.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DE3B40"));
                   
                });
            }
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

            // Set the new window's position and size to match the current window
            {
                WindowState = WindowState.Maximized // Set fullscreen
            };

            database.Show();

            this.Close();
        }

        private void btnSluiten_Click(object sender, RoutedEventArgs e)
        {
            MainWindow inloggen = new MainWindow()


            {
                WindowState = WindowState.Maximized // Set fullscreen
            };



            this.Close();
            inloggen.Show();

        }




        public static async Task SendDoorsturenRequest(string callerID)
        {
            var url = "https://lotjqigjud.a.pinggy.link/doorsturen"; // Domain and route for doorsturen

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("CallerID", callerID);

                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Successfully handled the closeCall request.");
                        MessageBox.Show("Het gesprek is succesvol `Doorgestuurd!", "Succes", MessageBoxButton.OK,
                            MessageBoxImage.Information);

                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        MessageBox.Show($"Fout bij sluiten gesprek: {response.StatusCode} - {response.ReasonPhrase}",
                            "Fout", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error making request: {ex.Message}");

                MessageBox.Show($"Er is een fout opgetreden: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public static async Task SendCloseCallRequest(string callerID)
        {
            var url = "https://lotjqigjud.a.pinggy.link/closeCall"; // The URL remains the same

            try
            {
                using (var client = new HttpClient())
                {
                    // Add the CallerID in the header
                    client.DefaultRequestHeaders.Add("CallerID", callerID);

                    // Send the GET request
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Successfully handled the closeCall request.");
                        MessageBox.Show("Het gesprek is succesvol gesloten!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        MessageBox.Show($"Fout bij sluiten gesprek: {response.StatusCode} - {response.ReasonPhrase}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                FirstError = true;

                Console.WriteLine($"Error making request: {ex.Message}");
                MessageBox.Show($"Er is een fout opgetreden: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpnemen_Click(object sender, RoutedEventArgs e)
        {
            SendDoorsturenRequest(CallerID);
        }

        private void btnOphangen_Click_1(object sender, RoutedEventArgs e)
        {
            SendCloseCallRequest(CallerID);
        }

        private void TxtGebruikernaam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                String Message = txtExternalBericht.Text;
                txtExternalBericht.Text = "";

                SendExternalMessageAsync(Message, CallerID);
            }
        }

        private void btnVersturen_Click(object sender, RoutedEventArgs e)
        {
            String Message = txtExternalBericht.Text;
            txtExternalBericht.Text = "";

            SendExternalMessageAsync(Message, CallerID);

        }
        public async void lastReceived()
        {

            var LastReceived = websocketController.GetDatetime();

            // Use the Dispatcher to update the UI on the main thread
            lblLastReceived.Dispatcher.Invoke(() =>
            {
                dataLoaded = true;
                lblLastReceived.Content = LastReceived.Result.ToString();
            });
        }
        public async Task SendExternalMessageAsync(string message, string callID)
        {
            try
            {

                // Build the URL with query parameters
                string url = $"https://lotjqigjud.a.pinggy.link/ExternalMessage?message={Uri.EscapeDataString(message)}&callID={Uri.EscapeDataString(callID)}";

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // If you need to read the response (in this case, a status code of 200)
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message successfully sent!");
                }
                else
                {
                    Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private async  void LoadData()
        {
            try
            {


                // Sample data from your JSON
                var callDataJson = await websocketController.GetJSONCallid(CallerID);

                if (callDataJson == "Niks")
                {
                    MessageBox.Show("No call data found.");
                    LoadData();
                }else{
                    // Deserialize the JSON string into CallData object
                    var callData = JsonConvert.DeserializeObject<CallData>(callDataJson);

                    // Populate the DataGrid with items
                    var items = new List<CallDataItem>
                    {
                       
                        new CallDataItem
                        {
                            CheckpointName = "GotName",
                            IsChecked = callData.Checkpoints.GotName,
                            DetailValue = callData.Voornaam ?? "Not provided"
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotEmail",
                            IsChecked = callData.Checkpoints.GotEmail,
                            DetailValue = callData.Email ?? "Not provided"
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotAdres",
                            IsChecked = callData.Checkpoints.GotAdres,
                            DetailValue = $"{callData.Postcode} {callData.Huisnummer}"
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotItem",
                            IsChecked = callData.Checkpoints.GotItem,
                            DetailValue = "Item details",
                            ShowButton = true
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotCategory",
                            IsChecked = callData.Checkpoints.GotCategory,
                            DetailValue = "Category details",
                            ShowButton = true
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotAvailabledates",
                            IsChecked = callData.Checkpoints.GotAvailableDates,
                            DetailValue = "Available dates",
                            ShowButton = true
                        },
                        new CallDataItem
                        {
                            CheckpointName = "GotSelectedDate",
                            IsChecked = callData.Checkpoints.GotSelectedDate,
                            DetailValue = callData.Afspraak.Naam ?? "Not selected"
                        },
                        new CallDataItem
                        {
                            CheckpointName = "PlannedDate",
                            IsChecked = callData.Checkpoints.PlannedDate,
                            DetailValue = "Planned date"
                        },
                        new CallDataItem
                        {
                        CheckpointName = "Doorgestuurd",
                        IsChecked = callData.Doorgestuurd,
                        DetailValue = callData.Doorgestuurd.ToString()
                        },
                    new CallDataItem
                    {
                        CheckpointName = "Closed",
                        IsChecked = callData.Closed,
                        DetailValue = callData.Closed.ToString()
                    },
                    };

                    // Update the DataGrid on the UI thread using Dispatcher
                    Dispatcher.Invoke(() =>
                    {
                        CallDataGrid.ItemsSource = items;
                    });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error loading data: {ex.Message}");
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }


        private void input_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            String Message = txtExternalBericht.Text;
            txtExternalBericht.Text = "";

            SendExternalMessageAsync(Message, CallerID);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to Button to access its properties
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Get the CheckpointName from the Tag property
                string checkpointName = clickedButton.Tag as string;
                MessageBox.Show($"Button clicked for checkpoint: {checkpointName}");
            }
        }

        private void btnJsonBekijken_Click(object sender, RoutedEventArgs e)
        {
             
                 // Assume this is the function where you get the JSON data
                 var callDataJson =  websocketController.GetJSONCallid(CallerID);

                 if (callDataJson != null && callDataJson.Result != "niks")
                 {
                     // Create an instance of the JsonPopupWindow and pass the JSON data to it
                     JsonMessage jsonMessage = new JsonMessage(callDataJson.Result);


                     jsonMessage.Width = 300;
                     jsonMessage.Height = 600;
                     // Show the window as a pop-up
                     jsonMessage.ShowDialog(); // This will open the window as a modal pop-up
                 }

        
        }

        private void txtExternalBericht_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private  void btnLuisteren_Click(object sender, RoutedEventArgs e)
        {
            if (ConversationClosed)
            {
                // Call the async method using Task.Run to execute it on a separate thread
                Task.Run(async () =>
                {
                    var callDataJson = await websocketController.GetJSONCallid(CallerID);

                    try
                    {

                        

                        // Parse the JSON string into a JObject
                        JObject jsonObject = JObject.Parse(callDataJson);

                        // Extract "Twillio" and "RecordingURL"
                        var recordingUrl = jsonObject["Twillio"]?["RecordingURL"].ToString(); // Safely access "Twillio"


                        if (recordingUrl != null && recordingUrl != "null")
                        {
                            string url = recordingUrl; // Your URL
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = url,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            MessageBox.Show("Geen recording URL");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                });

            }
            else
            {

            }
        }
        private void btnOpenBrowser_Click(object sender, RoutedEventArgs e)
        {
          
        }

        public class CallDataItem
        {
            public string CheckpointName { get; set; }
            public bool IsChecked { get; set; }
            public string DetailValue { get; set; }
            public bool ShowButton { get; set; }
        }

        public class CallData
        {
            public string Callinfo { get; set; }
            public bool Doorgestuurd { get; set; }
            public string Voornaam { get; set; }
            public string Email { get; set; }
            public string Postcode { get; set; }
            public string Huisnummer { get; set; }
            public bool? AdresGeldig { get; set; } // Changed to nullable bool
            public string Datetime { get; set; }
            public bool Closed { get; set; }
            public Checkpoints Checkpoints { get; set; }
            public Afspraak Afspraak { get; set; }
        }
        public class Checkpoints
        {
            public bool GotName { get; set; }
            public bool GotEmail { get; set; }
            public bool GotAdres { get; set; }
            public bool GotItem { get; set; }
            public bool GotCategory { get; set; }
            [JsonProperty("GotAvailabledates")] // Match JSON key exactly
            public bool GotAvailableDates { get; set; } // CamelCase for C# convention
            public bool GotSelectedDate { get; set; }
            public bool PlannedDate { get; set; }
        }

        public class Afspraak
        {
            public string Naam { get; set; }
        }


        
    }
}
