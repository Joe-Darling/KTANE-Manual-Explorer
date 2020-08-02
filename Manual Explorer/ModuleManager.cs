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
        private Dictionary<string, string> loadedModules = new Dictionary<string, string>();

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
            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Manual Config.txt"))
            {
                string[] pageConfigData = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Manual Config.txt");
                foreach(string config in pageConfigData)
                {
                    string readIn = config.Split(" -> ")[0];
                    string lookup = config.Split(" -> ")[1];
                    loadedModules.Add(readIn, lookup);
                }
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

        public bool DoesModuleExistInReadInModules(string moduleName)
        {
            return loadedModules.ContainsKey(moduleName.ToLower());
        }

        public string GetLookupString(string moduleName)
        {
            return loadedModules[moduleName.ToLower()];
        }

        public Dictionary<string, List<BitmapImage>>.KeyCollection GetModuleNames()
        {
            return modules.Keys;
        }

        public Dictionary<string, string>.KeyCollection GetReadInModuleNames()
        {
            return loadedModules.Keys;
        }

        public List<BitmapImage> GetManualPages(string manual)
        {
            return modules[manual];
        }

        public void AddLoadedModules(string[] mods)
        {
            foreach(string mod in mods)
            {
                AddLoadedModule(mod);
            }
        }

        public void AddLoadedModule(string mod)
        {
            mod = mod.ToLower();
            if (loadedModules.ContainsKey(mod))
            {
                return;
            }
            string value = modules.ContainsKey(mod) ? mod : "error page";
            loadedModules.Add(mod, value);
        }

        public void ChangeLookupForModule(string module, string newLookup)
        {
            module = module.ToLower();
            newLookup = newLookup.ToLower();
            if (loadedModules.ContainsKey(module.ToLower()))
            {
                loadedModules[module] = newLookup.ToLower();
            }
        }
    }
}
