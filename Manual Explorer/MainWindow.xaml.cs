using Microsoft.Win32;
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

namespace Manual_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<BitmapImage>> modules = new Dictionary<string, List<BitmapImage>>();
        private int tempIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateQuery(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            //if (e.Key == Key.Back)

            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                string userInput = comboBox.Text + e.Key.ToString().ToLower();
                comboBox.Items.Clear();

                List<string> contains = new List<string>();
                List<string> wordStarts = new List<string>();

                foreach (string nameToCheck in modules.Keys)
                {
                    if (nameToCheck.StartsWith(userInput))
                    {
                        comboBox.Items.Add(nameToCheck);
                    }

                    if (nameToCheck.Contains(" "))
                    {
                        string[] words = nameToCheck.Split(' ');
                        for (int i = 1; i < words.Length; i++)
                        {
                            if (words[i].StartsWith(userInput))
                            {
                                wordStarts.Add(nameToCheck);
                                //comboBox.Items.Add(nameToCheck);
                            }
                        }

                    }

                    else if (nameToCheck.Contains(userInput))
                    {
                        contains.Add(nameToCheck);
                        //combobox.items.add(nametocheck);
                    }
                }

                foreach (string moduleName in wordStarts)
                {
                    comboBox.Items.Add(moduleName);
                }

                foreach (string moduleName in contains)
                {
                    comboBox.Items.Add(moduleName);
                }

            }
            else if(e.Key == Key.LeftShift || e.Key == Key.Space)
            {

            }
            else
            {
                comboBox.Text = comboBox.Text.Substring(0, comboBox.Text.Length - 1);
                Trace.WriteLine("A letter was NOT pressed");
            }
            comboBox.IsDropDownOpen = true;
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(comboBox.SelectedItem != null)
            {
                LoadManual(comboBox.SelectedItem.ToString());
            }
            
        }

        private void ConvertPDFToImage(object sender, RoutedEventArgs e)
        {
            string initialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\341800";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.pdf", SearchOption.AllDirectories);

            Trace.WriteLine("Files:");
            foreach(string file in allFiles)
            {
                PdfDocument manualPdf = new PdfDocument();
                manualPdf.LoadFromFile(file);
                for (int page = 0; page < manualPdf.Pages.Count; page++)
                {
                    System.Drawing.Image bmp = manualPdf.SaveAsImage(page);
                    string[] pathBreakup = file.Split('\\');
                    bmp.Save("c:\\ManualHelper.Test\\" + pathBreakup[pathBreakup.Length - 1].Split('.')[0] + "-" + page + ".bmp");
                }
            }
        }

        private void InitializeActiveModules(object sender, RoutedEventArgs e)
        {
            // For now we will just initialize all modules.
            string initialDirectory = "C:\\ManualHelper.Test";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.bmp", SearchOption.AllDirectories);
            BitmapImage bitmap = new BitmapImage();
            if(modules.Keys.Count > 0)
            {
                Trace.WriteLine("Already initialized");
                return;
            }
            modules = new Dictionary<string, List<BitmapImage>>();

            foreach(string file in allFiles)
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                string moduleName = file.Substring(0, file.LastIndexOf('.') - 2);
                moduleName = moduleName.Split('\\').Last().ToLower();
                if (modules.ContainsKey(moduleName))
                {
                    modules[moduleName].Add(bitmap);
                }
                else
                {
                    modules[moduleName] = new List<BitmapImage>() { bitmap };
                }
                Trace.WriteLine(moduleName);
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

        private void LoadManual(string moduleName)
        {
            if (!modules.ContainsKey(moduleName))
            {
                throw new ArgumentException("This module name does not exist in the dictionary");
            }

            // TODO check if page is locked
            Page_1.Source = modules[moduleName][0];

            if (modules[moduleName].Count > 1)
            {
                Page_2.Source = modules[moduleName][1];
            }
            else
            {
                Page_2.Source = modules["blank page"][0];
            }
        }
    }
}
