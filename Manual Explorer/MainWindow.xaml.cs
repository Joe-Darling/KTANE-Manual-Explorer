﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // Comment
    public partial class MainWindow : Window
    {
        private string currentModule = string.Empty;
        private ProfileManager profileManager;
        private ModuleManager moduleManager;
        private HashSet<string> previouslySavedModules = new HashSet<string>();
        private string savePath = string.Empty;
        SearchFunctionality search = new SearchFunctionality();
        RightSideBarManager rightSideBarManager;

        public MainWindow()
        {
            InitializeComponent();
            moduleManager = ModuleManager.GetInstance();
            profileManager = new ProfileManager(History);
            rightSideBarManager = new RightSideBarManager(Serial_Number, AA_Count, D_Count, Battery_Holder_Count, Total_Battery_Count, DVI_Count, Parallel_Count, PS2_Count, RJ45_Count, Serial_Count,
                RCA_Count, Total_Port_Count, Total_Lit_Indicators, Total_Unlit_Indicators, Right_Panel);
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

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(comboBox.SelectedItem != null)
            {
                LoadManual(comboBox.SelectedItem.ToString());
            }
            
        }

        private void LoadRandomFile(object sender, RoutedEventArgs e)
        {
            string initialDirectory = "C:\\ManualHelper.Test";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.bmp", SearchOption.AllDirectories);
            Random rng = new Random();
            string file = allFiles[rng.Next(0, allFiles.Length - 1)];
            string moduleName = file.Substring(0, file.LastIndexOf('.') - 2);
            moduleName = moduleName.ToLower();
            moduleName = moduleName.Split('\\').Last();
            LoadManual(moduleName);
        }

        private void SaveCurrentModule(object sender, RoutedEventArgs e)
        {
            if (!History.Items.Contains(CapitilizeItem(currentModule)))
            {
                History.Items.Add(CapitilizeItem(currentModule));
            }
            History.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void DeleteCurrentModule(object sender, RoutedEventArgs e)
        {
            if (CapitilizeItem(currentModule) != string.Empty && History.Items.Contains(CapitilizeItem(currentModule)))
            {
                History.Items.Remove(CapitilizeItem(currentModule));
            }
        }

        private void LoadManual(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentModule = moduleName;
            if (!ModuleManager.GetInstance().DoesModuleExist(moduleName))
            {
                throw new ArgumentException("This module name does not exist in the dictionary");
            }

            // TODO check if page is locked
            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentModule);
            Page_1.Source = pages[0];

            if (pages.Count > 1)
            {
                Page_2.Source = pages[1];
            }
            else
            {
                Page_2.Source = ModuleManager.GetInstance().GetManualPages("blank page")[0];
            }
            
        }

        private void History_Selected(object sender, SelectionChangedEventArgs e)
        {
            ListBox comboBox = (ListBox)sender;
            if (comboBox.SelectedItem != null)
            {
                LoadManual(comboBox.SelectedItem.ToString());
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
    }
}
