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
        private string[] moduleNames = new string[] { "cheerleader", "hello world", "hello sofia", "test", "testing", "test123", "apple pie" };

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

                Trace.WriteLine(userInput);

                for (int i = 0; i < moduleNames.Length - 1; i++)
                {
                    if (moduleNames[i].Contains(userInput) == true)
                    {
                        comboBox.Items.Add(moduleNames[i]);
                    }


                }
            }
            else
            {

                comboBox.Text = comboBox.Text.Substring(0, comboBox.Text.Length - 1);
                Trace.WriteLine("A letter was NOT pressed");
            }

            //if (e.Key == Key.LeftShift)
            //{
            //    Trace.WriteLine("Shift has been used");
            //}

           

            
        }

        private void ConvertPDFToImage(object sender, RoutedEventArgs e)
        {
            string initialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\341800";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.pdf", SearchOption.AllDirectories);

            Trace.WriteLine("Files:");
            foreach(string file in allFiles)
            {
                Trace.WriteLine(file);
            }

            string filePath = "";
            PdfDocument manualPdf = new PdfDocument();
            manualPdf.LoadFromFile(filePath);
            System.Drawing.Image bmp = manualPdf.SaveAsImage(0);
            try
            {
                bmp.Save("c:\\ManualHelper.Test\\Test.bmp");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error saving your file: " + ex.Message);
            }
        }

        private void LoadImageOnScreen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            openFileDialog.ShowDialog();
            string filePath = openFileDialog.FileName;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            Temp_Image.Source = bitmap;
        }
    }
}
