using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts
{
    public class ServerManager : MonoBehaviour
    {
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;

        private bool _isServerStarted = false;
        private bool _isConnected = false;

        private const int Port = 8888;
        private const string Ip = "127.0.0.1";

        public GameManager GameManager { get; internal set; }

        private void Start() =>
            Debug.Log("Press space to start server");

        private async void StartServer()
        {
            Debug.Log("Starting server...");
            if (_isServerStarted)
                await StopServerAsync();

            _listener = new TcpListener(IPAddress.Parse(Ip), Port);
            _listener.Start();
            _isServerStarted = true;
            Debug.Log("Server started");

            _client = await _listener.AcceptTcpClientAsync();
            _isConnected = true;
            _stream = _client.GetStream();
        }

        private async Task StopServerAsync()
        {
            Debug.Log("Stopping server...");
            _stream?.Close();
            _client?.Close();
            _listener?.Stop();
            _isServerStarted = false;
            await Task.Delay(1000);
            Debug.Log("Server stopped");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartServer();

            if (_isServerStarted && _isConnected)
            {
                if (GameManager.IsResponseRequested)
                    return;

                if (_stream.DataAvailable)
                    Task.Run(async () => await ModelReceive());
            }
        }

        public async Task ModelReceive()
        {
            string message = (await ReceiveAsync()).Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            string[] splittedMessage = message?.Split(':') ?? new string[] { message };
            string funcName = splittedMessage?[0];

            GameManager.IsResponseRequested = true;

            if (funcName == "reset")
                GameManager.ResetGame();

            if (funcName == "set_action")
            {
                string argument = splittedMessage?[1];
                AgentAction action = (AgentAction)Enum.Parse(typeof(AgentAction), argument);
                GameManager.MakeAction(action);
            }

            if (funcName == "get_state")
                await SendAsync(GameManager.GetState().ToString());
        }

        public async Task ModelSend()
        {
            Debug.Log("ModelSend");
            GameManager.IsResponseRequested = false;

            // Get the reward from the Agent
            float reward = GameManager.GetReward();
            Debug.Log($"Reward : {reward}");
            Matrix<float> state = GameManager.GetState();
            Debug.Log($"State : {state}");
            bool isDone = GameManager.IsDone;
            Debug.Log($"IsDone : {isDone}");

            // Format the message
            string message = $"{reward.ToString(CultureInfo.InvariantCulture)}:{state}:{(isDone ? "1" : "0")}";

            //Debug.Log($"Message : {message}");
            // Send the message
            await SendAsync(message);

        }

        public async Task SendAsync(string message)
        {
            Debug.Log("Sending...");
            var data = Encoding.ASCII.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }

        public async Task<string> ReceiveAsync()
        {
            Debug.Log("Receiving...");
            string message;

            // Read from the stream until the delimiter '\n' is found
            do
            {
                var buffer = new List<byte>();
                var data = new byte[1];
                while (data[0] != '\n' && buffer.Count > 0)
                {
                    _stream.Read(data, 0, data.Length);
                    buffer.AddRange(data);
                }
                // Convert the data to a string
                message = Encoding.ASCII.GetString(buffer.ToArray());
            } while (string.IsNullOrWhiteSpace(message));

            Debug.Log("Received: " + message);
            return message;
        }
        private async void OnDestroy()
        {
            if (_isServerStarted)
            {
                await StopServerAsync();
            }
        }

    }
}