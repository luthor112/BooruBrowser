using BooruBrowser.Models;
using BooruBrowser.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
//using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BooruBrowser.Extensions
{
    [JsonObject()]
    public class IncrementalObservableCollection : ObservableCollection<IBooruImage>, ISupportIncrementalLoading
    {
        [JsonProperty("tags")]
        private string tags;
        [JsonProperty("next_page")]
        private int next_page;
        [JsonProperty("proxy")]
        private IBooruProxy proxy;

        [JsonProperty("HasMoreItems")]
        public bool HasMoreItems { get; set; }

        // Because of JSON
        [JsonProperty("$values")]
        private IList<IBooruImage> MyProperty {
            get
            {
                return this.Items;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            CoreDispatcher coreDispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(async () =>
            {
                List<IBooruImage> newItems;
                if (tags == null)
                    newItems = await proxy.GetNewestImages(50, next_page);
                else
                    newItems = await proxy.Search(50, next_page, tags);

                // Dispatcher: copy items to self, set next_page, set HasMoreItems
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (IBooruImage item in newItems)
                    {
                        this.Add(item);
                    }

                    if (newItems.Count == 0)
                    {
                        HasMoreItems = false;
                    }

                    next_page++;
                });

                return new LoadMoreItemsResult()
                {
                    Count = (uint)newItems.Count
                };
            }).AsAsyncOperation<LoadMoreItemsResult>();
        }

        public IncrementalObservableCollection(IBooruProxy proxy, string tags)
        {
            this.proxy = proxy;
            this.tags = tags;
            this.next_page = 1;
            this.HasMoreItems = true;
        }

        public IncrementalObservableCollection()
        {
            // Empty constructor needed for deserialization
        }
    }
}
