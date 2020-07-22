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

        //public void DisplayManualOneLocked(string moduleName)
        //{
        //    moduleName = moduleName.ToLower();
        //    currentManual = moduleName;
        //    if (!ModuleManager.GetInstance().DoesModuleExist(moduleName))
        //    {
        //        throw new ArgumentException("This module name does not exist in the dictionary");
        //    }

        //    List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

        //    if ()//leftPage page locked) MEANS: if LeftLock button was pressed
        //    {
        //        rightPage.Source = pages[0];
        //    }
        //    else if () // right page locked MEANS: if RightLock button was pressed
        //    {
        //        leftPage.Source = pages[0];
        //    }
        //    else //both locked
        //    {
        //        null;
        //    }
        //}

        public string GetCurrentActiveManual()
        {
            return currentManual;
        }

        public void TurnLeft(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                if (!rightPageC.EdgePageCheck(rightPage.Source))
                {
                    rightPage.Source = rightPageC.PreviousPage();
                }
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                if (!leftPageC.EdgePageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.PreviousPage();
                }
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (pages.Count > 2 && !leftPageC.EdgePageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.PreviousPage();
                    rightPage.Source = rightPageC.PreviousPage();
                }
            }
        }

        public void TurnRight(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                if (!rightPageC.EdgePageCheck(rightPage.Source))
                {
                    rightPage.Source = rightPageC.NextPage();
                }
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                if (!leftPageC.EdgePageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.NextPage();
                }
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (pages.Count > 2 && !rightPageC.EdgePageCheck(rightPage.Source))
                {
                    leftPage.Source = leftPageC.NextPage();
                    rightPage.Source = rightPageC.NextPage();
                }
            }




            //if (pages.Count > 2 && !rightPageC.EdgePageCheck(rightPage.Source))
            //{
            //    leftPage.Source = leftPageC.NextPage();
            //    rightPage.Source = rightPageC.NextPage();
            //}
        }

        public void LockLeft(Button leftLockBtn)
        {
            if (!leftPageC.Locked()) // if left lock is clicked
            {
                leftLockBtn.Content = "Unlock Left";
                leftLockBtn.Background = Brushes.IndianRed;
            }

            else  // if left page is unlocked
            {
                leftLockBtn.Content = "Lock Left";
                leftLockBtn.Background = Brushes.YellowGreen;
            }

            leftPageC.ChangeLockStatus();
        }

        public void LockRight(Button rightLockBtn)
        {
            if (!rightPageC.Locked()) // if right lock is clicked
            {
                rightLockBtn.Content = "Unlock Right";
                rightLockBtn.Background = Brushes.IndianRed;
            }

            else // if right page is unlocked
            {
                rightLockBtn.Content = "Lock Right";
                rightLockBtn.Background = Brushes.YellowGreen;
            }

            rightPageC.ChangeLockStatus();
        }
    }
}
