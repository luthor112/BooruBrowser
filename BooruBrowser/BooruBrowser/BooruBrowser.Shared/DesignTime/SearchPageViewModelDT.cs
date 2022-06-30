using BooruBrowser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BooruBrowser.DesignTime
{
    public class SearchPageViewModelDT
    {
        public SiteDescriptor CurrentDescriptor
        {
            get
            {
                return new SiteDescriptor()
                {
                    Name = "Site Name"
                };
            }
        }

        public string DefaultSearchString
        {
            get
            {
                return "tag_1 tag_2 tag_3 tag_4";
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
                };
            }
        }
    }
}
