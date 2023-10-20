﻿using System;
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
        }

        public async Task ModelSend()
        {
            // Get the reward from the Agent
            float reward = GameManager.GetReward();
            Matrix<float> state = GameManager.GetState();
            bool isDone = GameManager.IsDone;

            // Format the message
            string message = $"{reward.ToString(CultureInfo.InvariantCulture)}:{state}:{isDone}";

            // Send the message
            await SendAsync(message);

            GameManager.IsResponseRequested = false;
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
            // Read until the first pipe (|) character is encountered
            var buffer = new byte[16];
            var byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var message = Encoding.ASCII.GetString(buffer, 0, byteCount);
            // Get the message without the pipe character
            message = message.Substring(0, message.IndexOf("|", StringComparison.Ordinal));
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