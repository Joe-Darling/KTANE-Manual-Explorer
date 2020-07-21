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

        //public BitmapImage NextPage()
        //{
        //    //if (pageIndex < pages.Count - 1)
        //    //{
        //    //    pageIndex = pageIndex + 1;
        //    //    return pages[pageIndex];
        //    //}
        //    //else if (pages[pages.Count - 1] != blankPage)
        //    //{
        //    //    pages.Add(blankPage);
        //    //    return blankPage;
        //    //}
        //    //else
        //    //{
        //    //    return null;
        //    //}

        //    if (pageIndex < pages.Count - 1)
        //    {
        //        pageIndex = pageIndex + 1;
        //        return pages[pageIndex];
        //    }
        //    else
        //    {
        //        pages.Add(blankPage);
        //        return blankPage;
        //    }
        //}


        //if (pages[pages.Count - 1] == blankPage)
        //{
        //    if (pageIndex < pages.Count - 1)
        //    {
        //        pageIndex = pageIndex + 1;
        //        return pages[pageIndex];
        //    }
        //    else
        //    {
        //        return pages[pageIndex];
        //    }
        //}
        //else
        //{
        //    if (pageIndex < pages.Count - 1)
        //    {
        //        pageIndex = pageIndex + 1;
        //        return pages[pageIndex];
        //    }
        //    else
        //    {
        //        pages.Add(blankPage);
        //        return blankPage;
        //    }
        //} 
    

        //public BitmapImage PreviousPage()
        //{
        //    //if (pageIndex > 0)
        //    //{
        //        pageIndex = pageIndex - 1;
        //        return pages[pageIndex];
        //    //}
        //    //else
        //    //{
        //    //    return pages[pageIndex];
        //    //}
        //}

        public bool EdgePageCheck(ImageSource currPage)
        {
            if (currPage == pages[0] || currPage == blankPage)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public BitmapImage NextPageLeft()
        {
                pageIndex = pageIndex + 1;
                return pages[pageIndex];
        }

        public BitmapImage PreviousPage()
        {
            pageIndex = pageIndex - 1;
            return pages[pageIndex];
        }

        public BitmapImage NextPageRight()
        {
            if (pageIndex < pages.Count - 1)
            {
                pageIndex = pageIndex + 1;
                return pages[pageIndex];
            }
            else
            {
                pages.Add(blankPage);
                return pages[pageIndex + 1];
            }
        }

    }
}
