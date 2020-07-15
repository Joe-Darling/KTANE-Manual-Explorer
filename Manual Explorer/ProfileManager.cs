using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;

namespace Manual_Explorer
{
    class ProfileManager
    {
        private string currentProfilePath;
        private bool changeMade;
        private ListBox moduleList;
        private HashSet<string> previouslySavedModules;

        public ProfileManager(ListBox moduleList)
        {
            currentProfilePath = string.Empty;
            changeMade = false;
            previouslySavedModules = new HashSet<string>();
            this.moduleList = moduleList;
        }

        public string CapitilizeItem(string item)
        {
            return char.ToUpper(item[0]) + item.Substring(1);
        }

        private void SetPreviouslySavedModules()
        {
            previouslySavedModules = new HashSet<string>();
            foreach (string module in moduleList.Items)
            {
                previouslySavedModules.Add(module);
            }
        }

        public void NewProfile()
        {
            previouslySavedModules = new HashSet<string>();
            moduleList.Items.Clear();
        }

        public void OpenProfile()
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
                currentProfilePath = openFileDialog.FileName;
                try
                {
                    string[] modulesFromFile = System.IO.File.ReadAllText(currentProfilePath).Split('\n');
                    moduleList.Items.Clear();

                    foreach (string module in modulesFromFile)
                    {
                        ;
                        if (ModuleManager.GetInstance().DoesModuleExist(module))
                        {
                            moduleList.Items.Add(CapitilizeItem(module));
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

        public void SaveProfile()
        {
            if (changeMade)
            {
                if (currentProfilePath == string.Empty)
                {
                    SaveProfileAs();
                }
                else
                {
                    SaveFile(currentProfilePath);
                }
            }
        }

        public void SaveProfileAs()
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

            foreach (string module in moduleList.Items)
            {
                fileContent += module.ToLower() + "\n";
            }
            //Get the path of specified file
            currentProfilePath = filePath;
            try
            {
                System.IO.File.WriteAllText(currentProfilePath, fileContent);
                SetPreviouslySavedModules();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }
        }
    }
}
