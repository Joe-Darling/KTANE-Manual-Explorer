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
            // Downloads browser if one isn't already installed
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            // Launches new headless chrome browser
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            if(!TryGetKtaneSiteHtml(out string htmlString))
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

            // Foreach item in the html
            foreach (var child in directory.ChildNodes)
            {
                string manualUrlName = child.GetAttributeValue("link", "none");

                // If it is a manual page that is not a translation page
                if (child.Name.Equals("file") && !manualUrlName.Equals("none") && !child.InnerText.ToLower().Contains("translated"))
                {
                    Trace.Write("Manual: " + child.InnerText);
                    try
                    {
                        // Try to use the headless browser to go to that page, if successful download the PDF version of the page
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
                        // If an error occurs add it to list of failed files
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

        private bool TryGetKtaneSiteHtml(out string htmlString)
        {
            // Goes to ktane repo site and gets the html string from it
            htmlString = string.Empty;

            using (WebClient client = new WebClient())
            {
                htmlString = client.DownloadString("https://ktane.timwi.de/HTML/");
            }

            if (htmlString == string.Empty)
            {
                return false;
            }

            return true;
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
