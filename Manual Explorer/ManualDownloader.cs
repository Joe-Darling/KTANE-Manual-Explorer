using HtmlAgilityPack;
using PuppeteerSharp;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Manual_Explorer
{
    class ManualDownloader
    {
        private string downloadLocation;

        public ManualDownloader()
        {
            // update download location
        }

        private async void TestPDFDownloader()
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

            if (htmlString == string.Empty)
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
            foreach (string s in failedFiles)
            {
                Trace.WriteLine(s);
            }
        }

        private void ConvertPDFToImage()
        {
            //string initialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\workshop\\content\\341800";
            string initialDirectory = "C:\\Manual PDFs";
            string[] allFiles = Directory.GetFiles(initialDirectory, "*.pdf", SearchOption.AllDirectories);

            Trace.WriteLine("Files:");
            foreach (string file in allFiles)
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

        private void RenameFiles()
        {
            DirectoryInfo d = new DirectoryInfo("C:\\Tests");
            FileInfo[] infos = d.GetFiles();
            foreach (FileInfo f in infos)
            {
                File.Move(f.FullName, f.FullName.Replace(".html", ""));
            }
        }
    }
}
