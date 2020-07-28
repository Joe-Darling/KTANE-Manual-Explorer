using Microsoft.Win32;
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

        public MainWindow()
        {
            InitializeComponent();
            ModuleManager.GetInstance();
            profileManager = new ProfileManager(History);
            drawingManager = new DrawingManager();
            connectionWindow = new ConnectionWindow(this);
            connectionHandler = new ConnectionHandler(profileManager, Remaining_Time, Total_Modules, null, connectionWindow.Status_Text, connectionWindow.Room_ID_Text, connectionWindow.Password_Text);
            manualDisplayHandler = new ManualDisplayHandler(Page_1, Page_2);
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
            search.UpdateComboBox(comboBox, e, History);
        }

        private void BackspaceUpdate(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            search.UpdateComboBoxOnBackspace(comboBox, e, History);
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
                manualDisplayHandler.DisplayManual(comboBox.SelectedItem.ToString());
                ClearCheck();
            }
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(comboBox.SelectedItem != null)
            {
                manualDisplayHandler.DisplayManual(comboBox.SelectedItem.ToString());
                ClearCheck();
            }
            
        }

        private void SaveCurrentModule(object sender, RoutedEventArgs e)
        {
            string currentManual = CapitilizeItem(manualDisplayHandler.GetCurrentActiveManual());
            profileManager.AddToProfile(currentManual);
        }

        private void DeleteCurrentModule(object sender, RoutedEventArgs e)
        {
            string currentManual = CapitilizeItem(manualDisplayHandler.GetCurrentActiveManual());
            profileManager.DeleteFromProfile(currentManual);
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
            manualDisplayHandler.TurnLeft(manualDisplayHandler.GetCurrentActiveManual());
            ClearCheck();
        }

        private void PageTurnRight(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            manualDisplayHandler.TurnRight(manualDisplayHandler.GetCurrentActiveManual());
            ClearCheck();
        }
            
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            drawingManager.MouseButtonDown((Canvas)sender, e);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
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
            if (!manualDisplayHandler.isLocked("left"))
            {
                drawingManager.ClearPage(Left_Page_Drawing);
            }

            if (!manualDisplayHandler.isLocked("right"))
            {
                drawingManager.ClearPage(Right_Page_Drawing);
            }
        }

        private void EnterDrawingWindow(object sender, MouseEventArgs e)
        {
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
            Thread thread = new Thread(connectionHandler.ThreadStart);
            thread.Start();
        }
    }   
}
