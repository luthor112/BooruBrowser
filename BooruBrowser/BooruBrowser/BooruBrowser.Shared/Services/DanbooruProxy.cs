using BooruBrowser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace BooruBrowser.Services
{
    public class DanbooruProxy : IBooruProxy
    {
        private const string ServerUrl = "http://danbooru.donmai.us/";
        private const string ServerUrl_ = "http://danbooru.donmai.us";
        private static readonly string ImageURL = Path.Combine(ServerUrl, "posts/{0}.json");
        private static readonly string SearchURL = Path.Combine(ServerUrl, "posts.json?limit={0}&page={1}&tags={2}");
        private static readonly string NewestURL = Path.Combine(ServerUrl, "posts.json?limit={0}&page={1}");
        private static readonly string TagSearchURL = Path.Combine(ServerUrl, "tags.json?limit={0}&search[order]=count&search[name_matches]={1}*");

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
                DanbooruImage result = await GetRequestAsnyc<DanbooruImage>(string.Format(ImageURL, id));
                result.BigURL = ServerUrl_ + result.BigURL;
                result.MediumURL = ServerUrl_ + result.MediumURL;
                result.SmallURL = ServerUrl_ + result.SmallURL;
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IBooruImage>> Search(int limit, int page, string tags)
        {
            if (tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
            {
                return new List<IBooruImage>();
            }
            else
            {
                try
                {
                    List<DanbooruImage> searchResult = await GetRequestAsnyc<List<DanbooruImage>>(string.Format(SearchURL, limit, page, tags));
                    foreach (DanbooruImage image in searchResult)
                    {
                        image.BigURL = ServerUrl_ + image.BigURL;
                        image.MediumURL = ServerUrl_ + image.MediumURL;
                        image.SmallURL = ServerUrl_ + image.SmallURL;
                    }
                    return new List<IBooruImage>(searchResult);
                }
                catch
                {
                    return new List<IBooruImage>();
                }
            }
        }

        public async Task<List<IBooruImage>> GetNewestImages(int limit, int page)
        {
            try
            {
                List<DanbooruImage> searchResult = await GetRequestAsnyc<List<DanbooruImage>>(string.Format(NewestURL, limit, page));
                for (int i = 0; i < searchResult.Count; i++)
                {
                    if (searchResult[i].SmallURL == null || searchResult[i].SmallURL.Equals(""))
                    {
                        searchResult.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        searchResult[i].BigURL = ServerUrl_ + searchResult[i].BigURL;
                        searchResult[i].MediumURL = ServerUrl_ + searchResult[i].MediumURL;
                        searchResult[i].SmallURL = ServerUrl_ + searchResult[i].SmallURL;
                    }
                }
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
            else if (query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 2)
            {
                List<string> result = new List<string>();
                result.Add("You cannot search for more than 2 tags at a time!");
                return result;
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
                    List<DanbooruTag> searchResult = await GetRequestAsnyc<List<DanbooruTag>>(string.Format(TagSearchURL, 4, lastTag));
                    List<string> result = new List<string>();
                    foreach (DanbooruTag tag in searchResult)
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
