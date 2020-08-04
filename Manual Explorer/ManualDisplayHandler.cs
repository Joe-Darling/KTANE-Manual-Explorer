using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
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
        private DrawingManager drawingManager = new DrawingManager();
        public Canvas leftCanvas;
        public Canvas rightCanvas;

        public ManualDisplayHandler(Image leftPage, Image rightPage, Canvas leftCanvas, Canvas rightCanvas)
        {
            this.leftPage = leftPage;
            this.rightPage = rightPage;
            currentManual = string.Empty;
            leftPageC = new PageHandler(ModuleManager.GetInstance().GetManualPages("blank page"), 0, leftPage, currentManual);
            rightPageC = new PageHandler(ModuleManager.GetInstance().GetManualPages("blank page"), 0, rightPage, currentManual);
            this.leftCanvas = leftCanvas;
            this.rightCanvas = rightCanvas;
        }

        public void DisplayManual(string moduleName, bool connected)
        {

            moduleName = moduleName.ToLower();
            currentManual = moduleName;
            List<BitmapImage> pages = null;

            if (connected)
            {
                if (ModuleManager.GetInstance().DoesModuleExistInReadInModules(moduleName))
                {
                    string location = ModuleManager.GetInstance().GetLookupString(moduleName);
                    if (location.EndsWith(".html"))
                    {
                        Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", location);
                    }
                    else
                    {
                        pages = ModuleManager.GetInstance().GetManualPages(location);
                    }
                }
            }

            if (pages == null)
            {
                if (!ModuleManager.GetInstance().DoesModuleExist(moduleName))
                {
                    leftPage.Source = ModuleManager.GetInstance().GetManualPages("error page")[0];
                    rightPage.Source = ModuleManager.GetInstance().GetManualPages("error page")[0];
                    return;
                }
                else
                {
                    pages = ModuleManager.GetInstance().GetManualPages(currentManual);
                }
                
            }

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

        public ImageSource GetCurrentLeftPage()
        {
            return leftPageC.GetPageSource();
        }

        public ImageSource GetCurrentRightPage()
        {
            return rightPageC.GetPageSource();
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
            leftCanvas.Children.Clear();
            rightCanvas.Children.Clear();

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


            //leftCanvas.Children.Clear();
            //rightCanvas.Children.Clear();


            if (ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage()).Children.Count != 0)
            {
                var leftChildrenList = ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage()).Children.Cast<UIElement>().ToArray();
                ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage()).Children.Clear();
                if (ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage()) != new Canvas())
                {
                    foreach (var element in leftChildrenList)
                    {
                        leftCanvas.Children.Add(element);
                    }
                }
                else
                {
                    leftCanvas = ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage());
                }
                
            }

            if (ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentRightPage()).Children.Count != 0)
            {
                var rightChildrenList = ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentRightPage()).Children.Cast<UIElement>().ToArray();
                ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentRightPage()).Children.Clear();
                if (ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentRightPage()) != new Canvas())
                {
                    foreach (var element in rightChildrenList)
                    {
                        rightCanvas.Children.Add(element);
                    }
                }
                else
                {
                    rightCanvas = new Canvas();
                }
                
            }

            //var childrenLeft = ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentLeftPage()).Children;
            //var childrenRight = ModuleManager.GetInstance().WhichCanvasToUse(GetCurrentRightPage()).Children;
            //leftCanvas.Children.Clear();
            //rightCanvas.Children.Clear();
        }
    }
}
