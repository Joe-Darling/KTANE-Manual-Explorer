using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Manual_Explorer
{
    class ConnectionHandler
    {
        private ProfileManager profileManager;
        private TextBox remainingTime;
        private TextBox totalModules;
        private TextBox strikes;

        public ConnectionHandler(ProfileManager profileManager, TextBox remainingTime, TextBox totalModules, TextBox strikes)
        {
            this.profileManager = profileManager;
            this.remainingTime = remainingTime;
            this.totalModules = totalModules;
            this.strikes = strikes;
        }

        public void ThreadStart()
        {
            Trace.WriteLine("Thread started");
            CheckForFile();
        }

        private void CheckForFile()
        {
            string path = "C:\\Ktane Mod Builds\\test.txt";
            if (!File.Exists(path))
            {
                Thread.Sleep(1000);
                CheckForFile();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    profileManager.NewProfile();
                    string[] fileData = File.ReadAllLines(path);
                    remainingTime.Text = fileData[0].Split(' ')[1];
                    totalModules.Text = fileData[1].Split(' ')[2];
                    int ind = 5;
                    while (ind < fileData.Length)
                    {
                        string module = fileData[ind];
                        if (module == string.Empty)
                        {
                            break;
                        }
                        profileManager.AddToProfile(module);
                        ind++;
                    }
                });
                try
                {
                    File.Delete(path);
                    CheckForFile();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
