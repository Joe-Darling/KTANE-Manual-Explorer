using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;

namespace Manual_Explorer
{
    class PageInfo
    {
        //private BitmapImage currentLeftPage;
        //private BitmapImage currentRightPage;
        //private int currentLeftIndex;
        //private int currentRightIndex;
        //private string currentManual;
        private List<BitmapImage> pages;
        private int pageIndex;
        //public BitmapImage nextPage;
        //public BitmapImage previousPage;
        

        public PageInfo(List<BitmapImage> pages, int pageIndex)
        {
            this.pages = pages;
            this.pageIndex = pageIndex;
        }
        
        public BitmapImage nextPage()
        {
            pageIndex = pageIndex + 1;
            return pages[pageIndex];
        }

        public BitmapImage previousPage()
        {
            //BitmapImage previousPage;
            //if (currentIndex != 0)
            //{
            pageIndex = pageIndex - 1;
            return pages[pageIndex];
            //}

            //return previousPage;
        }


        //public int getLeftIndex(string moduleName)
        //{
        //    moduleName = moduleName.ToLower();
        //    currentManual = moduleName;

        //    List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);
        //    return pages.Count;
        //}

        //public int getRightIndex(string moduleName)
        //{
        //    moduleName = moduleName.ToLower();
        //    currentManual = moduleName;

        //    List<BitmapImage> pages = ModuleManager.GetInstance().GetManualPages(currentManual);
        //    return pages.Count;
        //}
    }
}
