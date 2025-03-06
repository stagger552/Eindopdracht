using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eindopdracht.Controller
{
    internal class WebsocketController
    {

        private ClientWebSocket _webSocket;
        private JObject _lastReceivedJson;
        private readonly object _lock = new object();
        private bool _isConnecting = false; // Prevents multiple connection attempts
        public WebsocketController()
        {
            _webSocket = new ClientWebSocket();
            Task.Run(() => ConnectAsync()); // Ensure connection starts when object is created
        }

        private async Task ConnectAsync()
        {
            if (_isConnecting || _webSocket.State == WebSocketState.Open)
                return; // Prevent duplicate connections

            _isConnecting = true;
            string websocketUrl = "wss://lotjqigjud.a.pinggy.link/LivaData";

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10))) // Timeout after 10s
                {
                    await _webSocket.ConnectAsync(new Uri(websocketUrl), cts.Token);
                    Console.WriteLine("✅ Connected to WebSocket.");
                }

                _ = Task.Run(() => ReceiveMessagesAsync()); // Start listening in the background
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("❌ WebSocket connection timed out.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ WebSocket Connection Error: {ex.Message}");
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[4096];
            StringBuilder messageBuilder = new StringBuilder();



            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string messageChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    messageBuilder.Append(messageChunk);

                    // Attempt to parse the accumulated message
                    try
                    {
                        JObject json = JObject.Parse(messageBuilder.ToString());
                        Console.WriteLine($"📩 Received: {json}");

                        // Store the latest JSON safely
                        _lastReceivedJson = json;

                        // Clear the StringBuilder for the next message
                        messageBuilder.Clear();
                    }
                    catch (JsonReaderException)
                    {
                        // JSON is incomplete; continue accumulating
                        continue;
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"⚠️ WebSocket Receive Error: {ex.Message}");
                    break; // Exit loop if WebSocket is broken
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error: {ex.Message}");
                }
            }

            await ReconnectAsync(); // Auto-reconnect
        }

        private async Task ReconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.Aborted)
            {
                Console.WriteLine("♻️ Reconnecting WebSocket...");
                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();
                await ConnectAsync();
            }
        }

        public  JObject GetLastReceivedJson()
        {
            lock (_lock)
            {
                return _lastReceivedJson;
            }
        }

        public async Task<int> GetActiveCalls()
        {
            var json = GetLastReceivedJson();
            if (json != null && json["type"]?.ToString() == "callData")
            {
                var activeCallsToken = json["data"]?["ActiveCalls"];
                if (activeCallsToken != null && int.TryParse(activeCallsToken.ToString(), out int activeCalls))
                {
                    return activeCalls;
                }
            }
            return -1;
        }

        public async Task<int> Get24Callls()
        {
            var json = GetLastReceivedJson();
            if (json != null && json["type"]?.ToString() == "callData")
            {
                var twentyFourCallsToken = json["data"]?["Calls24Hours"];
                if (twentyFourCallsToken != null && int.TryParse(twentyFourCallsToken.ToString(), out int twentyFourCalls))
                {
                    return twentyFourCalls;
                }
            }
            return 0;
        }

        public async Task<string> GetJSON()
        {
            var json = GetLastReceivedJson();
            if (json != null && json["type"]?.ToString() == "callData")
            {
                var jsonCallID = json["data"];


                return jsonCallID.ToString();
            }
            return "Niks";
        }

        public async Task<string> GetDatetime()
        {
            var json = GetLastReceivedJson();
            if (json != null && json["type"]?.ToString() == "callData")
            {
                var datetime = json["data"]?["Datetime"];


                return datetime.ToString();
            }
            return "Niks";
        }

        public async Task<string> GetJSONCallid(string CallID)
        {
            try
            {
                var json =  GetLastReceivedJson();
                if (json["data"]?["callId"]?[CallID] != null)
                {
                    var jsonCallID = json["data"]?["callId"]?[CallID];


                    return jsonCallID.ToString();
                }

                return "Niks";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Niks";
            }
         
        }
    }
}



