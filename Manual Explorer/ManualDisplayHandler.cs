using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class ManualDisplayHandler
    {
        private Image leftPage;
        private Image rightPage;
        private string currentManual;

        public ManualDisplayHandler(Image leftPage, Image rightPage)
        {
            this.leftPage = leftPage;
            this.rightPage = rightPage;
            currentManual = string.Empty;
        }

        public void DisplayManual(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;
            if (!ModuleManager.GetInstance().DoesModuleExist(moduleName))
            {
                throw new ArgumentException("This module name does not exist in the dictionary");
            }

            // TODO check if page is locked
            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);
            leftPage.Source = pages[0];

            if (pages.Count > 1)
            {
                rightPage.Source = pages[1];
            }
            else
            {
                rightPage.Source = ModuleManager.GetInstance().GetManualPages("blank page")[0];
            }
        }

        public string GetCurrentActiveManual()
        {
            return currentManual;
        }
    }
}
