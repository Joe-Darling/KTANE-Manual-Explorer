using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace Manual_Explorer
{
    class PageInfo
    {
        private List<BitmapImage> pages;
        private int pageIndex;
        private bool lockClicked = false; //page is unlocked by default
        //private ImageSource pageSource;

        public PageInfo(List<BitmapImage> pages, int pageIndex)
        {
            this.pages = pages;
            this.pageIndex = pageIndex;
        }

        public BitmapImage NextPage()
        {
            try
            {
                pageIndex += 1;
                return pages[pageIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                pageIndex -= 1;
                return pages[pageIndex];
            }
        }

        public BitmapImage PreviousPage()
        {
            try
            {
                pageIndex -= 1;
                return pages[pageIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                pageIndex += 1;
                return pages[pageIndex];
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
    }
}
