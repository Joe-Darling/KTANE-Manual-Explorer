using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class ManualDisplayHandler
    {
        private Image leftPage;
        private Image rightPage;
        private string currentManual;
        private PageInfo leftPageC;
        private PageInfo rightPageC;

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

            leftPageC = new PageInfo(pages, 0);
            rightPageC = new PageInfo(pages, 1);

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

        public void TurnLeft(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (pages.Count > 2 && !leftPageC.EdgePageCheck(leftPage.Source))
            {
                leftPage.Source = leftPageC.PreviousPage();
                rightPage.Source = rightPageC.PreviousPage();
            }
        }

        public void TurnRight(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);
            
            if (pages.Count > 2 && !rightPageC.EdgePageCheck(rightPage.Source))
            {
                leftPage.Source = leftPageC.NextPage();
                rightPage.Source = rightPageC.NextPage();
            }
        }
    }
}
