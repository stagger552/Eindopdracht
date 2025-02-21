using System;
using System.Collections.Generic;
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

        public GesprekDetails(String Callid)
        {
            timer = new System.Timers.Timer(1000); // 5000 milliseconds = 5 seconds
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            InitializeComponent();

            lblCallID.Content = Callid;
            CallerID = Callid;


        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoadTranscript();
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
                    string transcriptText = "";
                    foreach (var entry in fullTranscript)
                    {
                        transcriptText += $"{entry["role"]}: {entry["content"]}\n";
                    }
                    TranscriptTextBox.Dispatcher.Invoke(() =>
                        {
                            TranscriptTextBox.Text = transcriptText;

                        }
                    );

                }

              
                // Set the text of the TextBox
            }
        }

        async void btnOphangen_Copy_Click(object sender, RoutedEventArgs e)
        {
            ClientWebSocket ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://your-websocket-server.com:8080"), CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server");

            Console.Write("Enter Call ID to listen to: ");
            string callId = Console.ReadLine();

            string listenMessage = $"{{\"type\": \"listen\", \"callId\": \"{callId}\"}}";
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(listenMessage)), WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] buffer = new byte[1024];
            MemoryStream audioStream = new MemoryStream();
            WaveOutEvent waveOut = new WaveOutEvent();
            BufferedWaveProvider waveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            waveOut.Init(waveProvider);
            waveOut.Play();

            while (ws.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                audioStream.Write(buffer, 0, result.Count);
                waveProvider.AddSamples(buffer, 0, result.Count);
            }
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

    

        private void btnVersturen_Click(object sender, RoutedEventArgs e)
        {
            String Message = txtExternalBericht.Text;
            txtExternalBericht.Text = "";

            SendExternalMessageAsync(Message, CallerID);

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
    }
}
