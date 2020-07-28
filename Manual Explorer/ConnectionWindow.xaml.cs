using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {

        private MainWindow mainWindow;

        public ConnectionWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Tab)
            {
                return;
            }

            if(sender == Room_ID_Text)
            {
                Password_Text.Focus();
            }
            else
            {
                Join_Room_Button.Focus();
            }
        }

        private void AttemptToConnect(object sender, RoutedEventArgs e)
        {
            string roomID = string.Empty;
            string pass = string.Empty;

            Application.Current.Dispatcher.Invoke(() =>
            {
                roomID = Room_ID_Text.Text;
                pass = Password_Text.Text;
            });
            

            if(roomID == string.Empty || roomID.Equals("Enter room ID: (Required)")){
                Status_Text.Text = "The room ID cannot be empty. Please enter a valid room ID.";
                return;
            }
            Status_Text.Text = "Attempting to connect. Do not close this window";
            mainWindow.StartConnectionThread();
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if(textBox.Text.Equals("Enter room ID: (Required)") || textBox.Text.Equals("Enter password: (Optional)"))
            {
                textBox.Text = "";
            }
        }
    }
}
