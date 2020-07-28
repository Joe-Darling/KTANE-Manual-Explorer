using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace Manual_Explorer
{
    class PageHandler
    {
        private Image page;
        private List<BitmapImage> pages;
        private int pageIndex;
        private bool lockClicked = false; //page is unlocked by default
        private string currManual;
        //private ImageSource pageSource;

        public PageHandler(List<BitmapImage> pages, int pageIndex, Image page, string currManual) //, ImageSource pageSource
        {
            this.page = page;
            this.pages = pages;
            this.pageIndex = pageIndex;
            page.Source = pages[pageIndex];
            this.currManual = currManual;

            //this.pageSource = pageSource;
        }

        public ImageSource NextPage()
        {
            try
            {
                pageIndex += 1;
                page.Source = pages[pageIndex];
                return page.Source;
            }
            catch (ArgumentOutOfRangeException)
            {
                pageIndex -= 1;
                page.Source = pages[pageIndex];
                return page.Source;
            }
        }

        public ImageSource PreviousPage()
        {
            try
            {
                pageIndex -= 1;
                page.Source = pages[pageIndex];
                return page.Source;
            }
            catch (ArgumentOutOfRangeException)
            {
                pageIndex += 1;
                page.Source = pages[pageIndex];
                return page.Source;
            }
        }

        public bool EdgePageCheck(ImageSource currPage, int index)
        {
            return currPage == pages[index];
        }

        public int GetCurrIndex(ImageSource currPage)
        {
            int currIndex = 0;
            for (int i = 0; i < pages.Count; i++)
            {
                if (currPage == pages[i])
                {
                    currIndex = i;
                    break;
                }
            }
            return currIndex;
        }

        public bool Locked()
        {
            return lockClicked;
        }

        public void ChangeLockStatus()
        {
            lockClicked = !lockClicked; //if lockedClick is true set it to false and vice versa
        }

        public void LockPage(Button lockBtn)
        {
            if (!Locked()) // if lock is clicked
            {
                lockBtn.Content = "Unlock Left";
                lockBtn.Background = Brushes.IndianRed;
            }

            else  // if page is unlocked
            {
                lockBtn.Content = "Lock Left";
                lockBtn.Background = Brushes.YellowGreen;
            }
        }

        public void TurnPage(string direction, bool otherPageState)
        {
            if (direction.Equals("left") && otherPageState == true)
            {
                PreviousPage();
            }
            else if (direction.Equals("right") && otherPageState == true)
            {
                NextPage();
            }
        }

        public ImageSource GetPageSource()
        {
            return page.Source;
        }
        public ImageSource SetPageSource(ImageSource toSet)
        {
            page.Source = toSet;
            return page.Source;
        }

        public bool SameManual(PageHandler otherPage)
        {
            return currManual == otherPage.currManual;
        }
    }
}
