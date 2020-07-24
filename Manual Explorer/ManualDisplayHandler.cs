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

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            bool leftLockClicked = (leftPage.Source == null) ? false : leftPageC.Locked();
            bool rightLockClicked = (rightPage.Source == null) ? false : rightPageC.Locked();

            //bool sameManual = true;

            //if ((leftLockClicked && !rightLockClicked && rightPage.Source != null) || //left locked and manual changed
            //    (rightLockClicked && !leftLockClicked && rightPage.Source != null))   //right locked and manual changed
            //{
            //    sameManual = false;
            //}
            //else
            //{
            //    sameManual = true;
            //}

            //if (sameManual == true) //same manual on both pages
            //{

            //}
            //else //two different manuals on pages
            //{

            //}








            if (!leftLockClicked && !rightLockClicked)
            {
                leftPageC = new PageInfo(pages, 0);
                rightPageC = new PageInfo(pages, 1);

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
                rightPageC = new PageInfo(pages, 0);
                rightPage.Source = pages[0];
            }
            else if (rightLockClicked && !leftLockClicked)
            {
                leftPageC = new PageInfo(pages, 0);
                leftPage.Source = pages[0];
            }
        }

        public bool SameManual()
        {
            bool leftLockClicked = (leftPage.Source == null) ? false : leftPageC.Locked();
            bool rightLockClicked = (rightPage.Source == null) ? false : rightPageC.Locked();

            bool sameManual = true;

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

        public bool isLocked(string page)
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
            moduleName = moduleName.ToLower();
            currentManual = moduleName;

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();

            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                if (!rightPageC.FirstPageCheck(rightPage.Source))
                {
                    rightPage.Source = rightPageC.PreviousPage();
                }
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                if (!leftPageC.FirstPageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.PreviousPage();
                }
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (leftPageC.FirstPageCheck(leftPage.Source) && !rightPageC.SecondPageCheck(rightPage.Source) && !rightPageC.FirstPageCheck(rightPage.Source))
                {
                    rightPage.Source = rightPageC.PreviousPage();
                }
                else if (!leftPageC.FirstPageCheck(leftPage.Source) && !rightPageC.FirstPageCheck(rightPage.Source)) //ni jedna ni druga 
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

            bool sameManual = SameManual();
            Trace.WriteLine(sameManual);

            bool leftLockClicked = leftPageC.Locked();
            bool rightLockClicked = rightPageC.Locked();



            List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);

            if (leftLockClicked && !rightLockClicked) //only left page locked
            {
                if (!rightPageC.LastPageCheck(rightPage.Source))
                {
                    rightPage.Source = rightPageC.NextPage();
                }
            }
            else if (rightLockClicked && !leftLockClicked) //only right page locked 
            {
                if (!leftPageC.LastPageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.NextPage();
                }
            }
            else if (!leftLockClicked && !rightLockClicked) //both free
            {
                if (!leftPageC.FirstPageCheck(leftPage.Source) && !leftPageC.SecondLastPageCheck(leftPage.Source) && rightPageC.LastPageCheck(rightPage.Source))
                {
                    leftPage.Source = leftPageC.NextPage();
                }
                else if (!rightPageC.LastPageCheck(rightPage.Source) && !leftPageC.LastPageCheck(leftPage.Source))
                {
                    leftPage.Source = leftPageC.NextPage();
                    rightPage.Source = rightPageC.NextPage();
                }
            }
        }

        public void LockLeft(Button leftLockBtn)
        {   
            // check if the page is even there
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
