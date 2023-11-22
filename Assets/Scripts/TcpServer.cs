using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

using UnityEngine;

namespace Assets.Scripts
{
    public class TcpServer : MonoBehaviour
    {
        protected TcpListener tcpListener;
        protected Thread tcpListenerThread;
        protected TcpClient connectedTcpClient;
        protected bool isServerRunning = false;

        public GameManager GameManager { get; internal set; }

        public virtual void Awake()
        {
            GameManager = GetComponent<GameManager>();
        }

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartServer();
            }
        }

        public virtual void OnDestroy()
        {
            StopServer();
        }

        public virtual void StartServer()
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
            tcpListenerThread.Start();
            isServerRunning = true;
            Debug.Log("Server started.");
        }

        public virtual void StopServer()
        {
            isServerRunning = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListenerThread.Abort();
                Debug.Log("Server stopped.");
            }
        }

        public virtual void ListenForIncomingRequests()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052);
                tcpListener.Start();

                while (isServerRunning)
                {
                    // blocks until a client has connected to the server
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    // create a thread to handle communication with the connected client
                    Thread clientThread = new(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(tcpClient);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
        }

        public virtual void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            connectedTcpClient = tcpClient;

            NetworkStream clientStream = tcpClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;

            while (isServerRunning)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading from client: " + e.Message);
                    connectedTcpClient.Close();
                    return;
                }

                if (bytesRead == 0)
                {
                    Debug.LogWarning("Client disconnected.");
                    connectedTcpClient.Close();
                    return;
                }

                string receivedMessage = Encoding.ASCII.GetString(message, 0, bytesRead);

                // Check for the message terminator
                if (receivedMessage.Contains("\n"))
                {
                    // Message terminator found, process the complete message
                    receivedMessage = receivedMessage.Replace("\n", string.Empty);
                    ProcessReceivedMessage(receivedMessage);
                }
            }
        }

        public virtual void ProcessReceivedMessage(string message)
        {
            Debug.Log("Received: " + message);

            // Parse the received message and take appropriate actions
            string[] parts = message.Split(':');
            string command = parts[0];

            switch (command)
            {
                case "get_state":
                    SendData(GameManager.GetState().ToString());
                    break;
                case "reset":
                    GameManager.ResetGame();
                    break;
                case "set_action":
                    if (parts.Length == 2)
                    {
                        AgentAction action = (AgentAction)Enum.Parse(typeof(AgentAction), parts?[1]);
                        GameManager.MakeAction(action);
                    }
                    break;
                default:
                    Debug.LogWarning("Unknown command: " + command);
                    break;
            }
        }

        public virtual void SendData(string data)
        {
            if (connectedTcpClient == null)
            {
                Debug.LogError("No client connected.");
                return;
            }

            NetworkStream clientStream = connectedTcpClient.GetStream();
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            clientStream.Write(dataBytes, 0, dataBytes.Length);
            clientStream.Flush();
            Debug.Log("Sent: " + data);
        }
    }
}