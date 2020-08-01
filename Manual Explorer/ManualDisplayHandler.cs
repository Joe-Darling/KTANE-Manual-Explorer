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
        private PageHandler leftPageC;
        private PageHandler rightPageC;

        public ManualDisplayHandler(Image leftPage, Image rightPage)
        {
            this.leftPage = leftPage;
            this.rightPage = rightPage;
            currentManual = string.Empty;
            leftPageC = new PageHandler(ModuleManager.GetInstance().GetManualPages("blank page"), 0, leftPage, currentManual);
            rightPageC = new PageHandler(ModuleManager.GetInstance().GetManualPages("blank page"), 0, rightPage, currentManual);
        }

        public void DisplayManual(string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            if (!ModuleManager.GetInstance().DoesModuleExist(moduleName))
            {
                throw new ArgumentException("This module name does not exist in the dictionary");
            }

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();
            
            if (!leftLockClicked && !rightLockClicked)
            {
                if (pages.Count == 1)
                {
                    leftPageC = new PageHandler(pages, 0, leftPage, currentManual);
                    rightPageC = new PageHandler(ModuleManager.GetInstance().GetManualPages("blank page"), 0, rightPage, currentManual);
                }
                else
                {
                    leftPageC = new PageHandler(pages, 0, leftPage, currentManual);
                    rightPageC = new PageHandler(pages, 1, rightPage, currentManual);
                }
            }
            else if (leftLockClicked && !rightLockClicked) //left page locked
            {
                rightPageC = new PageHandler(pages, 0, rightPage, currentManual);
            }
            else if (rightLockClicked && !leftLockClicked)
            {
                leftPageC = new PageHandler(pages, 0, leftPage, currentManual);
            }
        }

        public bool IsLocked(string page)
        {
            if (page.Equals("left"))
            {
                return leftPageC.Locked();
            }
            else if (page.Equals("right"))
            {
                return rightPageC.Locked();
            }
            else
            {
                throw new ArgumentException("Value passed in has to be either \"left\" or \"right\"");
            }
        }

        public string GetCurrentActiveManual()
        {
            return currentManual;
        }

        public void TurnLeft(string moduleName)
        {
            TurnPage("left", moduleName);
        }

        public void TurnRight(string moduleName)
        {
            TurnPage("right", moduleName);
        }

        public void LockLeft(Button leftLockBtn)
        {
            if (leftPageC.GetPageSource() != null)
            {
                leftPageC.LockPage(leftLockBtn);
                leftPageC.ChangeLockStatus();
            }
        }

        public void LockRight(Button rightLockBtn)
        {
            if (rightPageC.GetPageSource() != null)
            {
                rightPageC.LockPage(rightLockBtn);
                rightPageC.ChangeLockStatus();
            }
        }

        public void TurnSame(List<BitmapImage> pages, string pagePosition)
        {
            if (leftPageC.GetCurrIndex(leftPage.Source) == (rightPageC.GetCurrIndex(rightPage.Source) - 1))
            {
                if (!leftPageC.EdgePageCheck(leftPageC.GetPageSource(), 0) && pagePosition.Equals("left"))
                {
                    leftPageC.PreviousPage();
                    rightPageC.PreviousPage();
                }
                else if (!leftPageC.EdgePageCheck(rightPageC.GetPageSource(), pages.Count - 1) && pagePosition.Equals("right"))
                {
                    leftPageC.NextPage();
                    rightPageC.NextPage();
                }
            }
            else if (leftPageC.GetCurrIndex(leftPageC.GetPageSource()) < rightPageC.GetCurrIndex(rightPageC.GetPageSource()))
            {
                switch (pagePosition)
                {
                    case "left":
                        rightPageC.PreviousPage();
                        break;
                    case "right":
                        leftPageC.NextPage();
                        break;
                }
            }
            else
            {   switch (pagePosition)
                {
                    case "left":
                        leftPageC.PreviousPage();
                        break;
                    case "right":
                        rightPageC.NextPage();
                        break;
                }
            }
        }

        public void TurnDiff(string pagePosition)
        {
            switch (pagePosition)
            {
                case "left":
                    leftPageC.PreviousPage();
                    rightPageC.PreviousPage();
                    break;
                case "right":
                    leftPageC.NextPage();
                    rightPageC.NextPage();
                    break;
            }
        }

        public void TurnPage(string turnDirection, string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                ImageSource pageToGet = turnDirection.Equals("left") ? rightPageC.PreviousPage() : rightPageC.NextPage();
                rightPageC.SetPageSource(pageToGet);
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                ImageSource pageToGet = turnDirection.Equals("left") ? leftPageC.PreviousPage() : leftPageC.NextPage();
                leftPageC.SetPageSource(pageToGet);
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (leftPageC.SameManual(rightPageC))
                {
                    TurnSame(pages, turnDirection);
                }
                else
                {
                    TurnDiff(turnDirection);
                }
            }
        }
    }
}
