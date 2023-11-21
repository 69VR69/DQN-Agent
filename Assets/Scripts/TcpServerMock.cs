using System.Collections.Generic;
using System.Threading;

using UnityEngine;

namespace Assets.Scripts
{
    public class TcpServerMock : TcpServer
    {
        [SerializeField]
        private List<string> simulatedReceivedDataList = new List<string>();

        private List<string> messageLogs = new List<string>();

        public override void StartServer()
        {
            isServerRunning = true;
            Debug.Log("Mock Server started.");
        }

        public override void StopServer()
        {
            isServerRunning = false;
            Debug.Log("Mock Server stopped.");
            DisplayMessageLogs();
        }

        public override void SendData(string data)
        {
            Debug.Log("Mock Sent: " + data);
            messageLogs.Add("Sent: " + data);
        }

        public override void HandleClientComm(object clientObj)
        {
            Debug.Log("Mock Client Connected.");

            foreach (string simulatedData in simulatedReceivedDataList)
            {
                // Process the simulated received data
                ProcessReceivedMessage(simulatedData);

                // Simulate a delay between messages (replace this with your own logic)
                Thread.Sleep(1000);
            }
        }

        public void DisplayMessageLogs()
        {
            Debug.Log("Message Logs:");
            foreach (string log in messageLogs)
            {
                Debug.Log(log);
            }
        }
    }
}