using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Linq;

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for PageConfigWindow.xaml
    /// </summary>
    public partial class PageConfigWindow : Window
    {
        SearchFunctionality search;

        public PageConfigWindow(SearchFunctionality search)
        {
            InitializeComponent();
            this.search = search;
        }

        private void UpdateQuery(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Selector nextFocus;
            if(e.Key == Key.Enter)
            {
                SetPageLookup(Read_In_Modules.Text, comboBox.Text);
                return;
            }
            List<string> keys;
            if(sender == Read_In_Modules)
            {
                nextFocus = Downloaded_Modules;
                keys = ModuleManager.GetInstance().GetReadInModuleNames().ToList();
            }
            else
            {
                nextFocus = Read_In_Modules;
                keys = ModuleManager.GetInstance().GetModuleNames().ToList();
            }
            search.UpdateComboBox(comboBox, e, nextFocus, keys);
        }

        private void BackspaceUpdate(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Selector nextFocus;
            if (sender == Read_In_Modules)
            {
                nextFocus = Downloaded_Modules;
            }
            else
            {
                nextFocus = Read_In_Modules;
            }
            search.UpdateComboBoxOnBackspace(comboBox, e, nextFocus, ModuleManager.GetInstance().GetModuleNames().ToList());
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                SetPageLookup(Read_In_Modules.Text, comboBox.SelectedItem.ToString());
            }
        }

        private void SetPageLookup(string readInManual, string thingToLookup)
        {
            ModuleManager.GetInstance().ChangeLookupForModule(readInManual, thingToLookup);
            Trace.WriteLine(readInManual + " set to " + thingToLookup);
        }

        private void SetDownloadedComboText(object sender, SelectionChangedEventArgs e)
        {
            if(Read_In_Modules.SelectedItem != null)
            {
                Downloaded_Modules.Text = ModuleManager.GetInstance().GetLookupString(Read_In_Modules.SelectedItem.ToString());
            }
        }
    }
}
