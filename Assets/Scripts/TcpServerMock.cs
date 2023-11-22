using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts
{
    public class TcpServerMock : TcpServer
    {
        [SerializeField]
        private List<string> simulatedReceivedDataList = new List<string>();
        private int currentDataIndex = 0;

        private List<string> messageLogs = new List<string>();

        public override void StartServer()
        {
            isServerRunning = true;
            Debug.Log("Mock Server started.");
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.N))
            {
                // Simulate receiving a message
                if (currentDataIndex < simulatedReceivedDataList.Count)
                {
                    string simulatedMessage = simulatedReceivedDataList[currentDataIndex];

                    // Process the simulated received data
                    ProcessReceivedMessage(simulatedMessage);

                    currentDataIndex++;
                }
                else
                {
                    Debug.Log("No more simulated messages.");
                }
            }
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

        public override void ProcessReceivedMessage(string message)
        {
            Debug.Log("Mock Received: " + message);
            messageLogs.Add("Received: " + message);
            // Add your logic here to handle the received message
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
