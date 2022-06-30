using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BooruBrowser.Models
{
    public class DanbooruTag : IBooruTag
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
