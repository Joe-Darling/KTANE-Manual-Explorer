using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Manual_Explorer
{
    class ConnectionHandler
    {
        private readonly string ip = "192.168.4.20";
        private readonly int port = 8080;
        private TcpClient tcpClient;
        private ProfileManager profileManager;
        private TextBox remainingTime;
        private TextBox totalModules;
        private TextBox strikes;
        private TextBlock statusText;
        private TextBox roomIDField;
        private TextBox passField;
        private Stream tcpStream;

        private ASCIIEncoding asen = new ASCIIEncoding();

        public ConnectionHandler(ProfileManager profileManager, TextBox remainingTime, TextBox totalModules, TextBox strikes, TextBlock statusText, TextBox roomIDField, TextBox passField)
        {
            this.profileManager = profileManager;
            this.remainingTime = remainingTime;
            this.totalModules = totalModules;
            this.strikes = strikes;
            this.statusText = statusText;
            this.roomIDField = roomIDField;
            this.passField = passField;
        }

        public void ThreadStart()
        {
            Trace.WriteLine("Thread started");
            string roomID = string.Empty;
            string pass = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                roomID = roomIDField.Text;
                pass = passField.Text;
            });

            bool connected = TryConnectToRoom(roomID, pass, out string response, out Exception exception);

            if (response == string.Empty)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    statusText.Text = "Could not connect to the server. Please check to ensure the server is online and you are connected to the internet.";
                });
                Trace.WriteLine(exception);
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                statusText.Text = response;
            });
                
            if (!connected)
            {
                return;
            }

            Trace.WriteLine("Connected, beginning thread loop");
            ThreadLoop();
        }

        public bool TryConnectToRoom(string roomID, string pass, out string response, out Exception exception)
        {
            if (string.IsNullOrEmpty(pass) || pass.Equals("Enter password: (Optional)"))
            {
                pass = "No Password";
            }
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
                tcpStream = tcpClient.GetStream();
                byte[] messageToSend = asen.GetBytes("join|" + roomID + "|" + pass);
                tcpStream.Write(messageToSend, 0, messageToSend.Length);

                byte[] responseBytes = new byte[100];
                tcpStream.Read(responseBytes, 0, 100);
                string[] serverMessage = asen.GetString(responseBytes).Split('|');
                response = serverMessage[1];
                exception = null;
                return serverMessage[0].Equals("true");
            }
            catch(Exception e)
            {
                exception = e;
                response = string.Empty;
                return false;
            }
        }

        public void ThreadLoop()
        {
            while (true)
            {
                byte[] newUpdate = new byte[10000];
                int bytesRead = tcpStream.Read(newUpdate, 0, newUpdate.Length);
                string[] serverMessage = asen.GetString(newUpdate, 0, bytesRead).Split('|');

                Console.WriteLine(bytesRead);
                switch (serverMessage[0])
                {
                    case "Start Level":
                        LoadNewLevel(serverMessage);
                        break;
                }
            }
        }

        public void LoadNewLevel(string[] levelParameters)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                profileManager.NewProfile();
                remainingTime.Text = levelParameters[1].Split(' ')[1];
                totalModules.Text = levelParameters[2].Split(' ')[2];
                int ind = 5;
                while (ind < levelParameters.Length)
                {
                    string module = levelParameters[ind];
                    if (string.IsNullOrEmpty(module))
                    {
                        break;
                    }
                    profileManager.AddToProfile(module);
                    ind++;
                }
            });
        }
    }
}
