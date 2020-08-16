using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
        private string roomID;
        private string roomPass;
        private bool connected;
        private bool alreadyConnected;

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

        public bool GetAlreadyConnected()
        {
            return alreadyConnected;
        }

        public void ThreadStart(string roomID, string pass, TextBlock statusText, bool reconnect)
        {
            Trace.WriteLine("Thread started");
            if (reconnect)
            {
                roomID = this.roomID;
                pass = roomPass;
            }

            bool connected = TryConnectToRoom(roomID, pass, out string response, out Exception exception);
            //if(connected && reconnect)
            //{
            //    return;
            //}

            if (response == string.Empty)
            {
                if (!reconnect)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        statusText.Text = "Could not connect to the server. Please check to ensure the server is online and you are connected to the internet.";
                    });
                }
                Trace.WriteLine(exception);
                return;
            }
            if (!reconnect)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    statusText.Text = response;
                });
            }
                
            if (!connected)
            {
                return;
            }

            Trace.WriteLine("Connected, beginning thread loop");
            //Thread reconnectCheckThread = new Thread(() => ConnectionCheckLoop());
            //reconnectCheckThread.Start();
            this.roomID = roomID;
            roomPass = pass;
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
                bool result = response.Split('|')[0].Equals("true");
                response = response.Split('|')[1];
                alreadyConnected = result;
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
            //File.WriteAllText("C:\\Ktane\\Client logs.txt", "The client thread loop was started at " + DateTime.Now + "\n\n");
            bool stillRunning = true;
            while (stillRunning)
            {
                if(!TryReadData(out var response, out Exception ex))
                {
                    MessageBox.Show("There was a problem reading in the string. " + ex.Message);
                    return;
                }
                

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
                    }
                }
            }
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
                MessageBox.Show(serverMessage, "Success!");
            });
        }

        private void LevelLost(string serverMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
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
            DateTime currentTime = DateTime.UtcNow;
            DateTime timeOfUpdate = DateTime.Parse(levelParameters[1], CultureInfo.CreateSpecificCulture("ru-RU"));
            TimeSpan timeLeft = TimeSpan.Parse(levelParameters[3]);
            timeLeft -= (currentTime - timeOfUpdate);

            Application.Current.Dispatcher.Invoke(() =>
            {
                profileManager.NewProfile();
                mainWindow.SetRemaningTimeText(timeLeft);
                totalModules.Text = levelParameters[5];
                int ind = 6;
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
            //File.AppendAllText("C:\\Ktane\\Client logs.txt", "Begun attempt to read message from host at " + DateTime.Now + "\n\n");

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
                    //File.AppendAllText("C:\\Ktane\\Client logs.txt", "We have read in " + totalBytesRead + "/" + messageLength + " so far.\n");

                    if (messageLength == -1 && Encoding.UTF8.GetString(responseBytes).Contains("|"))
                    {
                        messageLength = int.Parse(Encoding.UTF8.GetString(responseBytes).Split('|')[0]);
                        //File.AppendAllText("C:\\Ktane\\Client logs.txt", "The total length of this message is: " + messageLength + " bytes.\n");
                        if (messageLength == 0)
                        {
                            //File.AppendAllText("C:\\Ktane\\Client logs.txt", "Since the message length is 0, we bail out at " + DateTime.Now + "\n\n");
                            response = string.Empty;
                            return false;
                        }
                        headerBytes = messageLength.ToString().Length + 1;
                    }
                } while (totalBytesRead - headerBytes < messageLength);
                response = Encoding.UTF8.GetString(responseBytes, 0, totalBytesRead).Replace("\0", string.Empty);
                response = response.Substring(response.IndexOf("|") + 1);
                //File.AppendAllText("C:\\Ktane\\Client logs.txt", "Entire message was read in successfully.\nMessage: " + response);
                tcpStream.Flush();
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                response = string.Empty;
                //File.AppendAllText("C:\\Ktane\\Client logs.txt", "Failed attempt to read message from host at " + DateTime.Now + "\nReason: " + e.Message + "\n\n");
                tcpStream.Flush();
                return false;
            }
        }

        private void ConnectionCheckLoop()
        {
            while (true)
            {
                int failedAttempts = 0;

                try
                {
                    //Trace.WriteLine("Checking your connection");
                    TcpClient client = new TcpClient(ip, port);

                    IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                    TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();

                    if (tcpConnections != null && tcpConnections.Length > 0)
                    {
                        TcpState stateOfConnection = tcpConnections.First().State;
                        if (stateOfConnection != TcpState.Established)
                        {
                            // We are no longer connected. Attempt to reconnect.
                            Trace.WriteLine("You have disconnected from the client. Attempting to reconnect");
                            connected = AttemptToReconnect();
                            if (connected)
                            {
                                //Trace.WriteLine("Reconnected successfully");
                            }
                        }
                        else
                        {
                            //Trace.WriteLine("You are still connected");
                        }

                    }
                    client.Close();
                }
                catch(Exception e)
                {
                    failedAttempts++;
                    if(failedAttempts == 10)
                    {
                        MessageBox.Show("Unable to reconnect to client. Reason:" + e.Message);
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private bool AttemptToReconnect()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
                TrySendData("reconnect|" + roomID + "|" + roomPass + "|false");

                TryReadData(out string response, out Exception exception);
                bool result = response.Split('|')[0].Equals("true");
                response = response.Split('|')[1];
                return result;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
