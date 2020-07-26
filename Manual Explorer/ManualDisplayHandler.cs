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

            bool leftLockClicked = (leftPage.Source == null) ? false : leftPageC.Locked();
            bool rightLockClicked = (rightPage.Source == null) ? false : rightPageC.Locked();

            if (!leftLockClicked && !rightLockClicked)
            {
                leftPageC = new PageHandler(pages, 0, leftPage.Source);
                rightPageC = new PageHandler(pages, 1, rightPage.Source);

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
            else if (leftLockClicked && !rightLockClicked) //left page locked
            {
                rightPageC = new PageHandler(pages, 0, rightPage.Source);
                rightPage.Source = pages[0];
            }
            else if (rightLockClicked && !leftLockClicked)
            {
                leftPageC = new PageHandler(pages, 0, leftPage.Source);
                leftPage.Source = pages[0];
            }
        }

        public bool SameManual()
        {
            bool leftLockClicked = (leftPage.Source == null) ? false : leftPageC.Locked();
            bool rightLockClicked = (rightPage.Source == null) ? false : rightPageC.Locked();

            bool sameManual;

            if ((leftLockClicked && !rightLockClicked && rightPage.Source != null) || //left locked and manual changed
                (rightLockClicked && !leftLockClicked && rightPage.Source != null))   //right locked and manual changed
            {
                sameManual = false;
            }
            else
            {
                sameManual = true;
            }

            return sameManual;
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
            if (leftPage.Source != null)
            {
                leftPageC.LockPage(leftLockBtn);
                leftPageC.ChangeLockStatus();
            }
        }

        public void LockRight(Button rightLockBtn)
        {
            if (rightPage.Source != null)
            {
                rightPageC.LockPage(rightLockBtn);
                rightPageC.ChangeLockStatus();
            }
        }

        public void TurnSame(List<BitmapImage> pages, string pagePosition)
        {
            if (leftPageC.GetCurrIndex(leftPage.Source) == (rightPageC.GetCurrIndex(rightPage.Source) - 1))
            {
                if (!leftPageC.EdgePageCheck(leftPage.Source, 0) && pagePosition.Equals("left"))
                {
                    leftPage.Source = leftPageC.PreviousPage();
                    rightPage.Source = rightPageC.PreviousPage();
                }
                else if (!leftPageC.EdgePageCheck(rightPage.Source, pages.Count - 1) && pagePosition.Equals("right"))
                {
                    leftPage.Source = leftPageC.NextPage();
                    rightPage.Source = rightPageC.NextPage();
                }
            }
            else if (leftPageC.GetCurrIndex(leftPage.Source) < rightPageC.GetCurrIndex(rightPage.Source))
            {
                switch (pagePosition)
                {
                    case "left":
                        rightPage.Source = rightPageC.PreviousPage();
                        break;
                    case "right":
                        leftPage.Source = leftPageC.NextPage();
                        break;
                }
            }
            else
            {   switch (pagePosition)
                {
                    case "left":
                        leftPage.Source = leftPageC.PreviousPage();
                        break;
                    case "right":
                        rightPage.Source = rightPageC.NextPage();
                        break;
                }
            }
        }

        public void TurnDiff(string pagePosition)
        {
            switch (pagePosition)
            {
                case "left":
                    leftPage.Source = leftPageC.PreviousPage();
                    rightPage.Source = rightPageC.PreviousPage();
                    break;
                case "right":
                    leftPage.Source = leftPageC.NextPage();
                    rightPage.Source = rightPageC.NextPage();
                    break;
            }
        }

        public void TurnPage(string turnDirection, string moduleName)
        {
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            bool sameManual = SameManual();

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                ImageSource pageToGet = turnDirection.Equals("left") ? rightPageC.PreviousPage() : rightPageC.NextPage();
                rightPage.Source = pageToGet;
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                ImageSource pageToGet = turnDirection.Equals("left") ? leftPageC.PreviousPage() : leftPageC.NextPage();
                leftPage.Source = pageToGet;
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (SameManual())
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
