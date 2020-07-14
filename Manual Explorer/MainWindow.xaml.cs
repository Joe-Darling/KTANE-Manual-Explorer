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
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<BitmapImage>> modules = new Dictionary<string, List<BitmapImage>>();
        private string currentModule;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateQuery(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.Items.Clear();

            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                string userInput = comboBox.Text.ToLower() + e.Key.ToString().ToLower();

                for (int i = 0; i < 10; i++)
                {
                    if (userInput.Contains(i.ToString()))
                    {
                        userInput = userInput.Remove(userInput.Length - 2, 1);
                        break;
                    }
                }

                List<string> contains = new List<string>();
                List<string> wordStarts = new List<string>();

                foreach (string nameToCheck in modules.Keys)
                {
                    if (nameToCheck.StartsWith(userInput))
                    {
                        comboBox.Items.Add(CapitilizeItem(nameToCheck));
                    }

                    else if (nameToCheck.Contains(" "))
                    {
                        string[] words = nameToCheck.Split(' ');
                        for (int i = 1; i < words.Length; i++)
                        {
                            if (words[i].StartsWith(userInput))
                            {
                                wordStarts.Add(nameToCheck);
                            }
                        }
                    }

                    else if (nameToCheck.Contains(userInput))
                    {
                        contains.Add(nameToCheck);
                    }
                }


                foreach (string moduleName in wordStarts)
                {
                    comboBox.Items.Add(CapitilizeItem(moduleName));
                }

                foreach (string moduleName in contains)
                {
                    comboBox.Items.Add(CapitilizeItem(moduleName));
                }

                foreach (string item in modules.Keys)
                {
                    if (Compute(item, userInput) <= 3)
                    {
                        comboBox.Items.Add(CapitilizeItem(item));
                    }

                }

            }
            else if (e.Key == Key.LeftShift || e.Key == Key.Space || e.Key == Key.RightShift)
            {

            }
            else
            {
                e.Handled = true;
                Trace.WriteLine("A letter was NOT pressed");
            }

            comboBox.IsDropDownOpen = true;
        }

        public string CapitilizeItem(string item)
        {
            return char.ToUpper(item[0]) + item.Substring(1);
        }

        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
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

    }
}
