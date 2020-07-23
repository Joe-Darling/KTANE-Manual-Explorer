using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class PageInfo
    {
        private List<BitmapImage> pages;
        private int pageIndex;
        private static BitmapImage blankPage;
        private bool lockClicked = false; //page is unlocked by default

        public PageInfo(List<BitmapImage> pages, int pageIndex)
        {
            blankPage = ModuleManager.GetInstance().GetManualPages("blank page")[0];
            this.pages = pages;
            this.pageIndex = pageIndex;
        }

        public BitmapImage NextPage()
        {
            pageIndex = pageIndex + 1;
            return pages[pageIndex];
        }

        public BitmapImage PreviousPage()
        {
            pageIndex = pageIndex - 1;
            return pages[pageIndex];
        }

        public bool LastPageCheck(ImageSource currPage)
        {
            return currPage == pages[pages.Count - 1];
        }

        public bool FirstPageCheck(ImageSource currPage)
        {
            return currPage == pages[0]; 
        }

        public bool Locked()
        {
            return lockClicked;
        }

        public void ChangeLockStatus()
        {
            lockClicked = lockClicked ? false : true; //if lockedClick is true set it to false and vice versa
        }
    }
}
