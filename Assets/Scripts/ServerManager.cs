using System;
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
            _stream.Close();
            _client.Close();
            _listener.Stop();
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
                if (GameManager.Instance.ResponseRequested)
                    return;

                if (_stream.DataAvailable)
                    Task.Run(async () => await ModelReceive());
            }
        }

        public async Task ModelReceive()
        {
            string message = (await ReceiveAsync()).Trim();

            if (message == "reset")
                GameManager.Instance.ResetGame();

            string[] splittedMessage = message.Split(':');
            string funcName = splittedMessage[0];
            string argument = splittedMessage[1];

            if (funcName == "set_action")
            {
                AgentAction action = (AgentAction)Enum.Parse(typeof(AgentAction), argument);
                GameManager.Instance.MakeAction(action);
            }

            GameManager.Instance.ResponseRequested = true;
        }

        public async Task ModelSend()
        {
            // Get the reward from the Agent
            float reward = GameManager.Instance.GetReward();
            int state = GameManager.Instance.GetState();
            bool isDone = GameManager.Instance.IsDone;

            // Format the message
            string message = $"{reward.ToString(CultureInfo.InvariantCulture)}:{state}:{isDone}";

            // Send the message
            await SendAsync(message);

            GameManager.Instance.ResponseRequested = false;
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
            var data = new byte[256];
            var bytes = await _stream.ReadAsync(data, 0, data.Length);
            var message = Encoding.ASCII.GetString(data, 0, bytes);
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