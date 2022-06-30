using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Models
{
    public class DanbooruImage : IBooruImage
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("tag_string")]
        private string _tagString;
        public List<string> TagList
        {
            get
            {
                return new List<string>(_tagString.Split(new char[] { ' ' }));
            }
            set { }
        }
        [JsonProperty("uploader_name")]
        public string Uploader { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("score")]
        public int Score { get; set; }
        [JsonProperty("file_size")]
        public int FileSize { get; set; }
        [JsonProperty("large_file_url")]
        public string BigURL { get; set; }
        [JsonProperty("file_url")]
        public string MediumURL { get; set; }
        [JsonProperty("preview_file_url")]
        public string SmallURL { get; set; }
    }
}
