using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Models
{
    // WARNING: This class is for site-ignorant data sharing ONLY!
    // NO IMPLEMENTATION-SPECIFIC BEHAVIOUR ALLOWED!
    public class AnySiteImage : IBooruImage
    {
        public int ID { get; set; }
        public List<string> TagList { get; set; }
        public string Uploader { get; set; }
        public string Source { get; set; }
        public int Score { get; set; }
        public int FileSize { get; set; }
        public string BigURL { get; set; }
        public string MediumURL { get; set; }
        public string SmallURL { get; set; }
    }
}
