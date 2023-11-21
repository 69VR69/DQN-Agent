using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts
{
    public class ServerManagerMock : ServerManager
    {
        [SerializeField]
        private List<string> _orders = new();
        [SerializeField]
        private int _currentOrderIndex = 0;
        private readonly List<(string, string)> StreamMessages = new();
        private string CurrentSendingMessage = string.Empty;
        private string CurrentReceivingMessage = string.Empty;
        private bool isDataAvailable = true;

        private void AddLog(string message, bool isSendingMessage)
        {
            if (!string.IsNullOrWhiteSpace(CurrentReceivingMessage) && !string.IsNullOrWhiteSpace(CurrentReceivingMessage))
            {
                StreamMessages.Add((CurrentSendingMessage, CurrentReceivingMessage));
                CurrentSendingMessage = string.Empty;
                CurrentReceivingMessage = string.Empty;
                return;
            }

            if (isSendingMessage)
            {
                if (!string.IsNullOrWhiteSpace(CurrentSendingMessage))
                {
                    StreamMessages.Add((message, string.Empty));
                    CurrentSendingMessage = string.Empty;
                }
                else
                    CurrentSendingMessage = message;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(CurrentReceivingMessage))
                {
                    StreamMessages.Add((string.Empty, message));
                    CurrentReceivingMessage = string.Empty;
                }
                else
                    CurrentReceivingMessage = message;
            }
        }

        private byte[] GetBytesFromOrders()
        {
            var order = _orders[_currentOrderIndex] + '\n';
            _currentOrderIndex++;
            return Encoding.ASCII.GetBytes(order);
        }

        protected override void StartServer()
        {
            Debug.Log("Starting server...");
            if (_isServerStarted)
                StopServer();

            _isServerStarted = true;
            Debug.Log("Server started");

            _isConnected = true;
            isDataAvailable = true;
        }

        public override void SendAsync(string message)
        {
            Debug.Log("Sending...");
            //var data = Encoding.ASCII.GetBytes(message);
            Task.Delay(500);
            AddLog(message, true);
            isDataAvailable = true;
            Debug.Log("Sent: " + message);
        }

        public override string Receive()
        {
            Debug.Log("Receiving...");
            string message;

            // Read from the stream until the delimiter '\n' is found
            var stream = GetBytesFromOrders();

            var buffer = new List<byte>();
            var data = new byte[1];
            while (data[0] != '\n' && buffer.Count <= 0)
            {
                data = stream;
                Debug.Log($"Read the data : {data}");
                buffer.AddRange(data);
            }
            // Convert the data to a string
            message = Encoding.ASCII.GetString(buffer.ToArray());
            isDataAvailable = false;

            AddLog(message, false);

            Debug.Log("Received: " + message);

            return message;
        }

        protected override void StopServer()
        {
            Debug.Log("Stopping server...");
            _isServerStarted = false;
            Debug.Log("Server stopped");
            string data = StreamMessages.Select(s => $"[{s.Item2} -> {s.Item1}]").Aggregate((s1, s2) => s1 + '\n' + s2);
            Debug.Log("Result of the communication has been :");
            Debug.Log(data);
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartServer();

            if (_isServerStarted && _isConnected)
            {
                if (GameManager.IsFullAnswerRequested)
                    return;

                if (isDataAvailable)
                    Task.Run(() => ModelReceive());
            }

            if (_currentOrderIndex >= _orders.Count)
            {
                throw new System.Exception("end of mock simulation");
            }
        }
    }
}