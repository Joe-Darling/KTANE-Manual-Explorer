using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class ModuleManager
    {
        private static ModuleManager instance = null;
        private Dictionary<string, List<BitmapImage>> modules = new Dictionary<string, List<BitmapImage>>();

        private ModuleManager()
        {
            // For now we will just initialize all modules.
            string initialDirectory = "C:\\ManualHelper.Test";
            string secondaryDirectory = "F:\\V\\ManualHelper.Test";
            string[] allFiles;
            try
            {
                allFiles = Directory.GetFiles(initialDirectory, "*.bmp", SearchOption.AllDirectories);
            }
            catch(Exception e)
            {
               allFiles = Directory.GetFiles(secondaryDirectory, "*.bmp", SearchOption.AllDirectories);
            }
             
            BitmapImage bitmap = new BitmapImage();

            modules = new Dictionary<string, List<BitmapImage>>();

            foreach (string file in allFiles)
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                string moduleName = file.Substring(0, file.LastIndexOf('-'));
                moduleName = moduleName.Split('\\').Last().ToLower().Replace('’', '\'');
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

        public static ModuleManager GetInstance()
        {
            if(instance == null)
            {
                instance = new ModuleManager();
            }

            return instance;
        }

        public bool DoesModuleExist(string moduleName)
        {
            return modules.ContainsKey(moduleName);
        }

        public Dictionary<string, List<BitmapImage>>.KeyCollection GetModuleNames()
        {
            return modules.Keys;
        }

        public List<BitmapImage> GetManualPages(string manual)
        {
            return modules[manual];
        }
    }
}
