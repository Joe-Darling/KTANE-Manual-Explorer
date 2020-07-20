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
            //PageInfo pageObject = new 
            ImageSource currentLeft = leftPage.Source;
            ImageSource currentRight = rightPage.Source;

            


            int totalPages = pages.Count;
            int currentLeftIndex = 0;
            int currentRightIndex = 0;

            for (int i = 0; i < totalPages; i++)
            {
                if (currentLeft == pages[i])
                {
                    currentLeftIndex = i;
                }

                if (currentRight == pages[i])
                {
                    currentRightIndex = i;
                }
            }

            int firstPageIndex = 0;
            int lastPageIndex = pages.Count - 1;

            ImageSource firstPage = pages[firstPageIndex];
            ImageSource lastPage = pages[lastPageIndex];

            if (pages.Count > 2 && leftPage.Source != firstPage)
            {
                leftPage.Source = leftPageC.previousPage();
                rightPage.Source = rightPageC.previousPage();
                //leftPage.Source = pages[currentLeftIndex - 1];
                //rightPage.Source = pages[currentRightIndex - 1];
            }
            else
            {
                Trace.WriteLine("Left end reached");
            }

        }

        public void TurnRight(string moduleName)
        {

            //PageInfo PageObject = new PageInfo();
            //int leftIndex = PageObject.getLeftIndex();
            //int rightIndex = PageObject.getRightIndex();

            


            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            ImageSource currentLeft = leftPage.Source;
            ImageSource currentRight = rightPage.Source;


            int totalPages = pages.Count;
            int currentLeftIndex = 0;
            int currentRightIndex = 0;

            for (int i = 0; i < totalPages; i++)
            {
                if (currentLeft == pages[i])
                {
                    currentLeftIndex = i;
                }

                if (currentRight == pages[i])
                {
                    currentRightIndex = i;
                }
            }
            
            int firstPageIndex = 0;
            int lastPageIndex = pages.Count - 1;

            ImageSource firstPage = pages[firstPageIndex];
            ImageSource lastPage = pages[lastPageIndex];
            

            if (pages.Count > 2 && rightPage.Source != lastPage)
            {
                leftPage.Source = leftPageC.nextPage();
                rightPage.Source = rightPageC.nextPage();
                //leftPage.Source = pages[currentLeftIndex + 1];
                //rightPage.Source = pages[currentRightIndex + 1];
            }
            else
            {
                Trace.WriteLine("Right end reached");
            }
        }
    }
}
