using BooruBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BooruBrowser.DesignTime
{
    public class MainPageViewModelDT
    {
        public string MainPageTitle
        {
            get
            {
                return "Site Name";
            }
        }

        public List<IBooruImage> CurrentItemCollection
        {
            get
            {
                return new List<IBooruImage>()
                {
                    new AnySiteImage()
                    {
                        SmallURL = "ms-appx:///Assets/Logo.png"
                    },
                    new AnySiteImage()
                    {
                        SmallURL = "ms-appx:///Assets/Logo.png"
                    },
                    new AnySiteImage()
                    {
                        SmallURL = "ms-appx:///Assets/Logo.png"
                    },
                    new AnySiteImage()
                    {
                        SmallURL = "ms-appx:///Assets/Logo.png"
                    },
                    new AnySiteImage()
                    {
                        SmallURL = "ms-appx:///Assets/Logo.png"
                    },
                };
            }
        }
    }
}
