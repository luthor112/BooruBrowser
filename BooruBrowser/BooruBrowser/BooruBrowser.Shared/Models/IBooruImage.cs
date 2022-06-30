using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Models
{
    public interface IBooruImage
    {
        int ID { get; set; }
        List<string> TagList { get; set; }
        string Uploader { get; set; }
        string Source { get; set; }
        int Score { get; set; }
        int FileSize { get; set; }
        string BigURL { get; set; }
        string MediumURL { get; set; }
        string SmallURL { get; set; }
    }
}
