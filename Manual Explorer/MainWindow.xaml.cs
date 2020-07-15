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
        private Dictionary<string, List<BitmapImage>> modules = new Dictionary<string, List<BitmapImage>>();
        private string currentModule = string.Empty;
        private HashSet<string> previouslySavedModules = new HashSet<string>();
        private string savePath = string.Empty;
        SearchFunctionality search = new SearchFunctionality();

        public MainWindow()
        {
            InitializeComponent();
            InitializeActiveModules();
        }

        private void UpdateQuery(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            search.UpdateComboBox(comboBox, e, modules, History);
        }

        private void BackspaceUpdate(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            search.UpdateComboBoxOnBackspace(comboBox, e, modules, History);
        }
        private void NewProfile(object sender, RoutedEventArgs e)
        {
            previouslySavedModules = new HashSet<string>();
            History.Items.Clear();
        }

        private bool HaveChangesBeenMade()
        {
            if(History.Items.Count != previouslySavedModules.Count)
            {
                return true;
            }

            foreach(string module in History.Items)
            {
                if (!previouslySavedModules.Contains(module))
                {
                    return true;
                }
            }

            return false;
        }

        private void SetPreviouslySavedModules()
        {
            previouslySavedModules = new HashSet<string>();
            foreach(string module in History.Items)
            {
                previouslySavedModules.Add(module);
            }
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

        private void ConvertPDFToImage(object sender, RoutedEventArgs e)
        {
            //string initialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\341800";
            string initialDirectory = "C:\\Manual PDFs";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.pdf", SearchOption.AllDirectories);

            Trace.WriteLine("Files:");
            foreach(string file in allFiles)
            {
                Trace.WriteLine(file);
                PdfDocument manualPdf = new PdfDocument();
                manualPdf.LoadFromFile(file);
                for (int page = 0; page < manualPdf.Pages.Count; page++)
                {
                    System.Drawing.Image bmp = manualPdf.SaveAsImage(page);
                    string[] pathBreakup = file.Split('\\');
                    string filename = pathBreakup[pathBreakup.Length - 1];
                    filename = filename.Substring(0, filename.LastIndexOf('.'));
                    bmp.Save("c:\\ManualHelper.Test\\" + filename + "-" + page + ".bmp");
                }
            }
        }

        private void InitializeActiveModules()
        {
            // For now we will just initialize all modules.
            string initialDirectory = "F:\\V\\ManualHelper.Test";
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
                string moduleName = file.Substring(0, file.LastIndexOf('-'));
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

        private void SaveCurrentModule(object sender, RoutedEventArgs e)
        {
            if (!History.Items.Contains(CapitilizeItem(currentModule)))
            {
                History.Items.Add(CapitilizeItem(currentModule));
            }
            History.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void LoadManual(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentModule = moduleName;
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

        private void History_Selected(object sender, SelectionChangedEventArgs e)
        {
            ListBox comboBox = (ListBox)sender;
            if (comboBox.SelectedItem != null)
            {
                LoadManual(comboBox.SelectedItem.ToString());
            }
        }

        private void RenameFiles(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo("C:\\Tests");
            FileInfo[] infos = d.GetFiles();
            foreach (FileInfo f in infos)
            {
                File.Move(f.FullName, f.FullName.Replace(".html", ""));
            }
        }

        private async void TestPDFDownloader(object sender, RoutedEventArgs e)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            string htmlString = string.Empty;

            using (WebClient client = new WebClient())
            {
                htmlString = client.DownloadString("https://ktane.timwi.de/HTML/");
            }

            if(htmlString == string.Empty)
            {
                Trace.WriteLine("Unable to get list of modules from site.., aborting operation.");
                return;
            }

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlString);

            var directory = html.DocumentNode;
            var baseUrl = "https://ktane.timwi.de/HTML/";
            var failedFiles = new List<string>();
            directory = directory.SelectSingleNode("directory");
            foreach (var child in directory.ChildNodes)
            {
                string manualUrlName = child.GetAttributeValue("link", "none");

                if (child.Name.Equals("file") && !manualUrlName.Equals("none") && !child.InnerText.ToLower().Contains("translated"))
                {
                    Trace.Write("Manual: " + child.InnerText);
                    try
                    {
                        string manualName = child.InnerText.Replace(".html", "");
                        PuppeteerSharp.Page page = await browser.NewPageAsync();
                        await page.GoToAsync(baseUrl + manualUrlName);
                        PdfOptions pdfOptions = new PdfOptions();
                        pdfOptions.PrintBackground = true;
                        await page.PdfAsync("C:\\Tests\\" + manualName + ".pdf", pdfOptions);
                        Trace.WriteLine("Completed Successfully");
                    }
                    catch (Exception)
                    {
                        Trace.WriteLine("Failed to download correctly");
                        failedFiles.Add(child.InnerText);
                    }
                }
            }

            Trace.WriteLine("The following " + failedFiles.Count + " files failed: ");
            foreach(string s in failedFiles)
            {
                Trace.WriteLine(s);
            }
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                savePath = openFileDialog.FileName;
                try
                {
                    string[] moduleList = System.IO.File.ReadAllText(savePath).Split('\n');
                    History.Items.Clear();

                    foreach(string module in moduleList)
                    {
                        if (modules.ContainsKey(module))
                        {
                            History.Items.Add(CapitilizeItem(module));
                        }
                    }
                    SetPreviouslySavedModules();
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
            }
        }

        private void SaveProfile(object sender, RoutedEventArgs e)
        {
            if (HaveChangesBeenMade())
            {
                if(savePath == string.Empty)
                {
                    SaveProfileAs(sender, e);
                }
                else
                {
                    SaveFile(savePath);
                }
            }
        }

        private void SaveProfileAs(object sender, RoutedEventArgs e)
        {
            var fileContent = string.Empty;
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveFile(saveFileDialog.FileName);
            }
        }

        private void SaveFile(string filePath)
        {
            string fileContent = string.Empty;

            foreach (string module in History.Items)
            {
                fileContent += module.ToLower() + "\n";
            }
            //Get the path of specified file
            savePath = filePath;
            try
            {
                System.IO.File.WriteAllText(savePath, fileContent);
                SetPreviouslySavedModules();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }
        }

        private void DeleteCurrentModule(object sender, RoutedEventArgs e)
        {
            if(CapitilizeItem(currentModule) != string.Empty && History.Items.Contains(CapitilizeItem(currentModule)))
            {
                History.Items.Remove(CapitilizeItem(currentModule));
            }
        }
        private void ComboLostFocus(object sender, RoutedEventArgs e)
        {
            User_Query.IsDropDownOpen = false;
        }
    }
}
