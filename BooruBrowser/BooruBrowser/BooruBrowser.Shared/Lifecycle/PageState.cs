using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Lifecycle
{
    public class PageState
    {
        public object Parameter { get; set; }
        public Dictionary<string, object> CustomState { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool HasSavedState { get; set; }

        public TimeSpan Age
        {
            get { return DateTime.Now - CreatedDate; }
        }

        public static PageState CreateForSave(object parameter)
        {
            PageState pageState = new PageState
            {
                Parameter = parameter,
                CreatedDate = DateTime.Now,
                CustomState = new Dictionary<string, object>(),
                HasSavedState = true
            };

            return pageState;
        }
    }
}
