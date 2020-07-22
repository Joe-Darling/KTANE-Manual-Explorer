using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class PageInfo
    {
        private List<BitmapImage> pages;
        private int pageIndex;
        private static BitmapImage blankPage;

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

        public bool EdgePageCheck(ImageSource currPage)
        {
            if (currPage == pages[0] || currPage == pages[pages.Count -1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
