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
        protected TcpListener _listener;
        protected TcpClient _client;
        protected NetworkStream _stream;

        protected bool _isServerStarted = false;
        protected bool _isConnected = false;

        protected const int Port = 8888;
        protected const string Ip = "127.0.0.1";

        public GameManager GameManager { get; internal set; }

        protected void Start() =>
            Debug.Log("Press space to start server");

        protected virtual void StartServer()
        {
            Debug.Log("Starting server...");
            if (_isServerStarted)
                StopServer();

            _listener = new TcpListener(IPAddress.Parse(Ip), Port);
            _listener.Start();
            _isServerStarted = true;
            Debug.Log("Server started");

            _client = Task.Run(async () => await _listener.AcceptTcpClientAsync()).Result;
            _isConnected = true;
            _stream = _client.GetStream();
        }

        protected virtual void StopServer()
        {
            Debug.Log("Stopping server...");
            _stream?.Close();
            _client?.Close();
            _listener?.Stop();
            _isServerStarted = false;
            Task.Delay(1000);
            Debug.Log("Server stopped");
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartServer();

            if (_isServerStarted && _isConnected)
            {
                if (GameManager.IsFullAnswerRequested)
                    return;

                if (_stream.DataAvailable)
                    ModelReceive();
            }
        }

        public virtual void ModelReceive()
        {
            string message = (Receive()).Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            string[] splittedMessage = message?.Split(':') ?? new string[] { message };
            string funcName = splittedMessage?[0];

            Debug.Log($"FuncName : {funcName} and message : {message}");

            if (funcName == "reset")
                GameManager.ResetGame();

            if (funcName == "set_action")
            {
                GameManager.IsFullAnswerRequested = true;
                string argument = splittedMessage?[1];
                AgentAction action = (AgentAction)Enum.Parse(typeof(AgentAction), argument);
                GameManager.MakeAction(action);
            }

            if (funcName == "get_state")
                SendAsync(GameManager.GetState().ToString());
        }

        public virtual void ModelSend()
        {
            Debug.Log("ModelSend");
            GameManager.IsFullAnswerRequested = false;

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
            SendAsync(message);

        }

        public virtual void SendAsync(string message)
        {
            Debug.Log("Sending...");
            var data = Encoding.ASCII.GetBytes(message);
            _stream.WriteAsync(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }

        public virtual string Receive()
        {
            Debug.Log("Receiving...");
            string message;

            // Read from the stream until the delimiter '\n' is found
            var buffer = new List<byte>();
            var data = new byte[1];
            while (data[0] != '\n' && buffer.Count <= 0)
            {
                _stream.Read(data, 0, data.Length);
                buffer.AddRange(data);
            }
            // Convert the data to a string
            message = Encoding.ASCII.GetString(buffer.ToArray());

            Debug.Log("Received: " + message);
            return message;
        }
        protected virtual void OnDestroy()
        {
            if (_isServerStarted)
            {
                StopServer();
            }
        }

    }
}