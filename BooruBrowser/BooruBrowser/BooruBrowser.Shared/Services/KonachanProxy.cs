using BooruBrowser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace BooruBrowser.Services
{
    public class KonachanProxy : IBooruProxy
    {
        private const string ServerUrl = "http://konachan.com/";
        private static readonly string SearchURL = Path.Combine(ServerUrl, "post.json?limit={0}&page={1}&tags={2}");
        private static readonly string NewestURL = Path.Combine(ServerUrl, "post.json?limit={0}&page={1}");
        private static readonly string TagSearchURL = Path.Combine(ServerUrl, "tag.json?limit={0}&order=count&name={1}");

        private async Task<T> GetRequestAsnyc<T>(string uri)
        {
            using (var client = new HttpClient())
            {
                string json = await client.GetStringAsync(new Uri(uri));
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        public async Task<IBooruImage> GetImage(int id)
        {
            try
            {
                List<KonachanImage> searchResult = await GetRequestAsnyc<List<KonachanImage>>(string.Format(SearchURL, 1, 0, "id:" + id.ToString()));

                if (searchResult.Count == 1)
                    return searchResult[0];
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IBooruImage>> Search(int limit, int page, string tags)
        {
            try
            {
                List<KonachanImage> searchResult = await GetRequestAsnyc<List<KonachanImage>>(string.Format(SearchURL, limit, page, tags));
                return new List<IBooruImage>(searchResult);
            }
            catch
            {
                return new List<IBooruImage>();
            }
        }

        public async Task<List<IBooruImage>> GetNewestImages(int limit, int page)
        {
            try
            {
                List<KonachanImage> searchResult = await GetRequestAsnyc<List<KonachanImage>>(string.Format(NewestURL, limit, page));
                return new List<IBooruImage>(searchResult);
            }
            catch
            {
                return new List<IBooruImage>();
            }
        }

        public async Task<List<string>> GetSearchSuggestions(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new List<string>();
            }
            else if (query[query.Length - 1] == ' ')
            {
                return new List<string>();
            }
            else
            {
                string lastTag;
                string beforeLastTag;
                if (query.Contains(" "))
                {
                    lastTag = query.Substring(query.LastIndexOf(' ') + 1);
                    beforeLastTag = query.Substring(0, query.LastIndexOf(' ') + 1);
                }
                else
                {
                    lastTag = query;
                    beforeLastTag = "";
                }

                try
                {
                    List<KonachanTag> searchResult = await GetRequestAsnyc<List<KonachanTag>>(string.Format(TagSearchURL, 4, lastTag));
                    List<string> result = new List<string>();
                    foreach (KonachanTag tag in searchResult)
                    {
                        result.Add(beforeLastTag + tag.Name);
                    }
                    return result;
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
    }
}
