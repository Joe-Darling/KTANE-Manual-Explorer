using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly string ip = "176.99.158.28";
        private readonly int port = 8080;
        private TcpClient tcpClient;
        private ProfileManager profileManager;
        private MainWindow mainWindow;
        private TextBox remainingTime;
        private TextBox totalModules;
        private TextBox strikes;
        private Stream tcpStream;

        public ConnectionHandler(MainWindow mainWindow, ProfileManager profileManager, TextBox remainingTime, TextBox totalModules, TextBox strikes)
        {
            this.mainWindow = mainWindow;
            this.profileManager = profileManager;
            this.remainingTime = remainingTime;
            this.totalModules = totalModules;
            this.strikes = strikes;
        }

        public TcpClient GetTcpClient()
        {
            return tcpClient;
        }

        public void ThreadStart(string roomID, string pass, TextBlock statusText)
        {
            Trace.WriteLine("Thread started");
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
                TrySendData("join|" + roomID + "|" + pass);

                TryReadData(out response, out exception);
                exception = null;
                bool result = response.Split('|')[0].Equals("true");
                response = response.Split('|')[1];

                return result;
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
            bool stillRunning = true;
            while (stillRunning)
            {
                TryReadData(out var response, out var ex);

                string[] serverMessage = response.Split('|');
                if(serverMessage.Length > 1)
                {
                    switch (serverMessage[0])
                    {
                        case "Start Level":
                            LoadNewLevel(serverMessage);
                            break;
                        case "Close Room":
                            RoomClosed();
                            stillRunning = false;
                            break;
                        case "Complete Level":
                            LevelComplete(serverMessage[1]);
                            break;
                        case "Lost Level":
                            LevelLost(serverMessage[1]);
                            break;
                        case "Hosts Modules":
                            SetHostsModules(serverMessage);
                            break;
                        case "Timer Update":
                            TimerUpdate(serverMessage);
                            break;
                    }
                }
            }
        }

        private void TimerUpdate(string[] serverMessage)
        {
            DateTime currentTime = DateTime.UtcNow;
            DateTime timeMessageWasGenerated = DateTime.Parse(serverMessage[1]);
            TimeSpan timeLeft = TimeSpan.Parse(serverMessage[2]);
            int rateOfChange = int.Parse(serverMessage[3]);

            // Remaning time = base remaning time minus the time it takes to send and recieve the message + 1 (rounding up to the nearest second)
            timeLeft -= (currentTime - timeMessageWasGenerated) + TimeSpan.FromSeconds(1);
            Trace.WriteLine(timeLeft);

            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.SetRemaningTimeText(timeLeft);
                mainWindow.AdustTimerSpeed(rateOfChange);
            });
        }

        private void SetHostsModules(string[] modules)
        {
            modules = modules.Skip(1).ToArray();
            ModuleManager.GetInstance().AddLoadedModules(modules);
        }

        private void LevelComplete(string serverMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.StopTimer();
                MessageBox.Show(serverMessage, "Success!");
            });
        }

        private void LevelLost(string serverMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.StopTimer();
                MessageBox.Show(serverMessage, "Uh-Oh.");
            });
        }

        private void RoomClosed()
        {
            tcpClient.Close();
            tcpClient = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("The host has closed the connection", "Connection closed");
            });
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
                    if (!ModuleManager.GetInstance().DoesModuleExistInReadInModules(module))
                    {
                        ModuleManager.GetInstance().AddLoadedModule(module);
                    }
                    profileManager.AddToProfile(module);
                    ind++;
                }
            });
        }

        public bool TrySendData(string message)
        {
            try
            {
                int messageLength = Encoding.UTF8.GetBytes(message).Length;
                byte[] messageToSend = Encoding.UTF8.GetBytes(messageLength.ToString() + "|" + message);
                tcpStream = tcpClient.GetStream();
                tcpStream.Write(messageToSend, 0, messageToSend.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryReadData(out string response, out Exception exception)
        {
            exception = null;
            try
            {
                byte[] responseBytes = new byte[20000];
                int totalBytesRead = 0;
                int messageLength = -1;
                int headerBytes = 0;
                do
                {
                    totalBytesRead += tcpStream.Read(responseBytes, totalBytesRead, responseBytes.Length - totalBytesRead);
                    if (messageLength == -1 && Encoding.UTF8.GetString(responseBytes).Contains("|"))
                    {
                        messageLength = int.Parse(Encoding.UTF8.GetString(responseBytes).Split('|')[0]);
                        headerBytes = messageLength.ToString().Length + 1;
                    }
                } while (totalBytesRead - headerBytes < messageLength);
                response = Encoding.UTF8.GetString(responseBytes, 0, totalBytesRead).Replace("\0", string.Empty);
                response = response.Substring(response.IndexOf("|") + 1);

                return true;
            }
            catch (Exception e)
            {
                exception = e;
                response = string.Empty;
                return false;
            }
        }
    }
}
