using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
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
using Microsoft.VisualBasic;
using NAudio.Wave;


namespace Eindopdracht.View
{
    /// <summary>
    /// Interaction logic for GesprekDetails.xaml
    /// </summary>
    public partial class GesprekDetails : Window
    {
        public GesprekDetails()
        {
            InitializeComponent();
            LoadMenuForm();
        }

        private void LoadMenuForm()
        {
            // Create instance of the menu form
            Menu menu = new Menu();

            // Extract the content from the MenuForm
            var menuContent = menu.Content;



            // Add it to our DockPanel's ContentControl
            MenuFormContainer.Content = menuContent;
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
    }
}
