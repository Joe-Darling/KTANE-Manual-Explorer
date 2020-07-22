using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
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

        public bool ShouldClose()
        {
            if (changeMade)
            {
                MessageBoxResult result = MessageBox.Show("You have unsaved changes to your current profile. Would you like to save them?", "Save?", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        return SaveProfile();
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;
                    default:
                        throw new ArgumentException("Invalid result: " + result);
                }
            }
            else
            {
                return true;
            }
           
        }

        private void SetPreviouslySavedModules()
        {
            previouslySavedModules = new HashSet<string>();
            foreach (string module in moduleList.Items)
            {
                previouslySavedModules.Add(module);
            }
            changeMade = false;
        }

        public void AddToProfile(string currentManual)
        {
            if (!moduleList.Items.Contains(currentManual))
            {
                moduleList.Items.Add(currentManual);
            }
            moduleList.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
            changeMade = true;
        }

        public void DeleteFromProfile(string currentManual)
        {
            if (currentManual != string.Empty && moduleList.Items.Contains(currentManual))
            {
                moduleList.Items.Remove(currentManual);
            }
            changeMade = true;
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

        public bool SaveProfile()
        {
            if (changeMade)
            {
                if (currentProfilePath == string.Empty)
                {
                    return SaveProfileAs();
                }
                else
                {
                    SaveFile(currentProfilePath);
                    return true;
                }
            }
            return true;
        }

        public bool SaveProfileAs()
        {
            var fileContent = string.Empty;
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            bool choseFile = saveFileDialog.ShowDialog() ?? false;
            if (choseFile)
            {
                SaveFile(saveFileDialog.FileName);
            }
            return choseFile;
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
