﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Spire.Pdf;
using System.Diagnostics;
using System.Net;
using PuppeteerSharp;
using HtmlAgilityPack;
using System.Threading;
using System.ComponentModel;

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProfileManager profileManager;
        ConnectionHandler connectionHandler;
        SearchFunctionality search = new SearchFunctionality();
        RightSideBarManager rightSideBarManager;
        ManualDisplayHandler manualDisplayHandler;
        DrawingManager drawingManager;
        ConnectionWindow connectionWindow;
        PageConfigWindow pageConfigWindow;
        Thread connectionThread;

        public MainWindow()
        {
            InitializeComponent();
            ModuleManager.GetInstance();
            profileManager = new ProfileManager(History);
            drawingManager = new DrawingManager();
            manualDisplayHandler = new ManualDisplayHandler(Page_1, Page_2, Left_Page_Drawing, Right_Page_Drawing);
            connectionWindow = new ConnectionWindow(this);
            connectionHandler = new ConnectionHandler(this, profileManager, Remaining_Time, Total_Modules, null);
            rightSideBarManager = new RightSideBarManager(Serial_Number, AA_Count, D_Count, Battery_Holder_Count, Total_Battery_Count, DVI_Count, Parallel_Count, PS2_Count, RJ45_Count, Serial_Count,
                RCA_Count, Total_Port_Count, Total_Lit_Indicators, Total_Unlit_Indicators, Right_Panel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!profileManager.ShouldClose())
            {
                e.Cancel = true;
            }
        }

        private void UpdateQuery(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            search.UpdateComboBox(comboBox, e, History, ModuleManager.GetInstance().GetModuleNames().ToList());
        }

        private void BackspaceUpdate(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            search.UpdateComboBoxOnBackspace(comboBox, e, History, ModuleManager.GetInstance().GetModuleNames().ToList());
        }

        public string CapitilizeItem(string item)
        {
            return char.ToUpper(item[0]) + item.Substring(1);
        }

        private void History_Selected(object sender, SelectionChangedEventArgs e)
        {
            ListBox comboBox = (ListBox)sender;
            if (comboBox.SelectedItem != null)
            {
                ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
                manualDisplayHandler.DisplayManual(comboBox.SelectedItem.ToString(), connectionHandler.GetTcpClient() != null);
                manualDisplayHandler.CanvasLoader();
                //ClearCheck();
            }
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(comboBox.SelectedItem != null)
            {
                ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
                manualDisplayHandler.DisplayManual(comboBox.SelectedItem.ToString(), connectionHandler.GetTcpClient() != null);
                manualDisplayHandler.CanvasLoader();
                //ClearCheck();
            }
        }

        private void SaveCurrentModule(object sender, RoutedEventArgs e)
        {
            string currentManual = CapitilizeItem(manualDisplayHandler.GetCurrentActiveManual());
            profileManager.AddToProfile(currentManual);
        }

        private void DeleteCurrentModule(object sender, RoutedEventArgs e)
        {
            if(History.SelectedItem != null)
            {
                string currentManual = History.SelectedItem.ToString();
                profileManager.DeleteFromProfile(currentManual);
            }
        }

        private void NewProfile(object sender, RoutedEventArgs e)
        {
            profileManager.NewProfile();
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            profileManager.OpenProfile();
        }

        private void SaveProfile(object sender, RoutedEventArgs e)
        {
            profileManager.SaveProfile();
        }

        private void SaveProfileAs(object sender, RoutedEventArgs e)
        {
            profileManager.SaveProfileAs();
        }

        private void OpenConnectionWindow(object sender, RoutedEventArgs e)
        {
            connectionWindow = new ConnectionWindow(this);
            connectionWindow.Show();
        }

        private void ComboLostFocus(object sender, RoutedEventArgs e)
        {
            User_Query.IsDropDownOpen = false;
        }

        private void ResetRightSideBar(Object sender, RoutedEventArgs e)
        {
            rightSideBarManager.ResetPanel();
        }

        private void ChangeQuantity(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string buttonName = button.Name;
            string[] terms = buttonName.Split('_');

            int amount = terms[0].Equals("ADD") ? 1 : -1;
            WidgetType type = (terms[1].Equals("AA") || terms[1].Equals("D")) ? WidgetType.BATTERY : WidgetType.PORT;
            rightSideBarManager.ChangeQuantity(terms[1], amount, type);
        }

        private void SelectedSerialNumberBox(object sender, RoutedEventArgs e)
        {
            rightSideBarManager.SelectedSerialNumberBox();
        }

        private void ToggleIndicator(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            rightSideBarManager.ToggledIndicator(checkBox.Name.Split('_')[0].Equals("LIT"), checkBox.IsChecked.Value);
        }

        private void PageTurnLeft(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
            //drawingManager.ClearPage(Left_Page_Drawing);
            //drawingManager.ClearPage(Right_Page_Drawing);
            //ClearCheck();
            manualDisplayHandler.TurnLeft(manualDisplayHandler.GetCurrentActiveManual());
            manualDisplayHandler.CanvasLoader();
            //ClearCheck();
        }

        private void PageTurnRight(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
            //drawingManager.ClearPage(Left_Page_Drawing);
            //drawingManager.ClearPage(Right_Page_Drawing);
            //ClearCheck();
            manualDisplayHandler.TurnRight(manualDisplayHandler.GetCurrentActiveManual());
            manualDisplayHandler.CanvasLoader();
            

            //ClearCheck();
        }
            
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Canvas canvas = drawingManager.WhichCanvasToUse(manualDisplayHandler.GetCurrentLeftPage(), (Canvas)sender);
            drawingManager.MouseButtonDown((Canvas)sender, e);
            
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            //Canvas canvas = drawingManager.WhichCanvasToUse(manualDisplayHandler.GetCurrentLeftPage(), (Canvas)sender);
            drawingManager.MouseMove((Canvas)sender, e);
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            drawingManager.MouseButtonUp(e);
        }

        private void ClearDrawing(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name.Equals("Left_Clear"))
            {
                drawingManager.ClearPage(Left_Page_Drawing);
            }
            else
            {
                drawingManager.ClearPage(Right_Page_Drawing);
            }
        }

        private void ClearCheck()
        {
            if (!manualDisplayHandler.IsLocked("left"))
            {
                drawingManager.ClearPage(Left_Page_Drawing);
            }

            if (!manualDisplayHandler.IsLocked("right"))
            {
                drawingManager.ClearPage(Right_Page_Drawing);
            }
        }

        private void EnterDrawingWindow(object sender, MouseEventArgs e)
        {   
            //Canvas canvas = drawingManager.WhichCanvasToUse(manualDisplayHandler.GetCurrentLeftPage(), (Canvas)sender);
            drawingManager.OnMouseEnter((Canvas)sender, e);
        }

        private void LockLeftPage(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            manualDisplayHandler.LockLeft(button);
        }

        private void LockRightPage(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            manualDisplayHandler.LockRight(button);
        }

        public void StartConnectionThread()
        {
            string roomID = connectionWindow.Room_ID_Text.Text;
            string password = connectionWindow.Password_Text.Text;
            TextBlock statusText = connectionWindow.Status_Text;
            connectionThread = new Thread(() => connectionHandler.ThreadStart(roomID, password, statusText, false));
            connectionThread.Start();
        }

        private bool TryOpenPageConfigWindow()
        {
            if (connectionHandler.GetTcpClient() == null)
            {
                MessageBox.Show("You need to be connected to a room before you can configure pages.");
                return false;
            }

            pageConfigWindow = new PageConfigWindow(search);
            pageConfigWindow.Show();
            return true;
        }

        private void ConfigurePages(object sender, RoutedEventArgs e)
        {
            TryOpenPageConfigWindow();
        }

        private void ConfigureManualPage(object sender, RoutedEventArgs e)
        {
            if (TryOpenPageConfigWindow())
            {
                pageConfigWindow.SetReadInComboText(History.SelectedItem.ToString());
            }
        }
        
        private void ArrowControl(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == Key.Left)
            {
                //PageTurnLeft(sender, e);
                ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
                manualDisplayHandler.TurnLeft(manualDisplayHandler.GetCurrentActiveManual());
                manualDisplayHandler.CanvasLoader();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == Key.Right)
            {
                //PageTurnRight(sender, e);
                ModuleManager.GetInstance().CheckToSave(Left_Page_Drawing, Right_Page_Drawing, manualDisplayHandler.GetCurrentLeftPage(), manualDisplayHandler.GetCurrentRightPage());
                manualDisplayHandler.TurnRight(manualDisplayHandler.GetCurrentActiveManual());
                manualDisplayHandler.CanvasLoader();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == Key.Q) // left lock
            {
                manualDisplayHandler.LockLeft(lockLeftBtn);
            }
            else if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && e.Key == Key.W) // right lock
            {
                manualDisplayHandler.LockRight(lockRightBtn);
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.Key == Key.R)
            {
                if (connectionHandler.GetAlreadyConnected())
                {
                    connectionThread = new Thread(() => connectionHandler.ThreadStart(null, null, null, true));
                    connectionThread.Start();
                    Trace.WriteLine("Attempting Reconnect");
                }
                else
                {
                    OpenConnectionWindow(null, null);
                    Trace.WriteLine("Attempting Inital Connect");
                }
            }
        }

        public void SetRemaningTimeText(TimeSpan remaningTime)
        {
            StringBuilder timeText = new StringBuilder();

            if (remaningTime.Days > 0)
            {
                timeText.Append(remaningTime.Days + ":");
            }
            if (remaningTime.Hours > 0)
            {
                timeText.Append(remaningTime.Hours + ":");
            }

            timeText.Append(remaningTime.Minutes + ":" + remaningTime.Seconds);

            Remaining_Time.Text = timeText.ToString();
        }
    }   
}
