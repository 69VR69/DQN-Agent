using System.Collections;
using System.Net;
using System.Net.Sockets;

using UnityEngine;

namespace Assets.Scripts
{
    public class ServerManager : MonoBehaviour
    {
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;

        private const int Port = 8888;
        private const string Ip = "127.0.0.1";

        private void Start()
        {
            _listener = new TcpListener(IPAddress.Parse(Ip), Port);
            _listener.Start();
            Debug.Log("Server started");
            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
        }

        private void Update()
        {
            if (_stream.DataAvailable)
            {
                var message = Receive();
                if (message == "Hello")
                {
                    Send("Hi");
                }
            }
        }

        public void Send(string message)
        {
            var data = System.Text.Encoding.ASCII.GetBytes(message);
            _stream.Write(data, 0, data.Length);
            Debug.Log("Sent: " + message);
        }

        public string Receive()
        {
            var data = new byte[256];
            var bytes = _stream.Read(data, 0, data.Length);
            var message = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: " + message);
            return message;
        }

        private void OnDestroy()
        {
            _stream.Close();
            _client.Close();
            _listener.Stop();
        }
    }
}