using BooruBrowser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.DesignTime
{
    public class ImagePageViewModelDT
    {
        public SiteDescriptor CurrentSiteDescriptor
        {
            get
            {
                return new SiteDescriptor()
                {
                    Name = "Site Name"
                };
            }
        }

        public IBooruImage CurrentImageData
        {
            get
            {
                return new AnySiteImage()
                {
                    MediumURL = "ms-appx:///Assets/Logo.png",
                    TagList = new List<string>() { "Tag_1", "Tag_2", "Tag_3", "Tag_4" },
                    Uploader = "Name of the uploader",
                    Source = "",
                    FileSize = 1234,
                    Score = 1234
                };
            }
        }
    }
}
