using BooruBrowser.Extensions;
using BooruBrowser.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Models
{
    public class SiteDescriptor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public IBooruProxy Proxy { get; set; }
        public IncrementalObservableCollection NewestItems { get; set; }
    }
}
