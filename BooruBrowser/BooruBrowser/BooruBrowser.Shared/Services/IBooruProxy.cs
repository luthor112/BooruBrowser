using BooruBrowser.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BooruBrowser.Services
{
    public interface IBooruProxy
    {
        /*async*/ Task<IBooruImage> GetImage(int id);
        /*async*/ Task<List<IBooruImage>> Search(int limit, int page, string tags);
        /*async*/ Task<List<IBooruImage>> GetNewestImages(int limit, int page);
        /*async*/ Task<List<string>> GetSearchSuggestions(string query);
    }
}
