using BooruBrowser.Extensions;
using BooruBrowser.Models;
using BooruBrowser.MVVM;
using BooruBrowser.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace BooruBrowser.ViewModels
{
    public partial class MainPageViewModel
    {
        partial void PartialConstructor()
        {
            SuggestionsRequestedCommand = new DelegateCommand<SearchBoxSuggestionsRequestedEventArgs>(SuggestionsRequested);
            ResultSuggestionChosenCommand = new DelegateCommand<SearchBoxResultSuggestionChosenEventArgs>(SuggestionClicked);
            QuerySubmittedCommand = new DelegateCommand<SearchBoxQuerySubmittedEventArgs>(SwitchToSearchPage);
        }

        private void SwitchToSearchPage(SearchBoxQuerySubmittedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.QueryText))
                return;

            // Minimize serialized data on suspend
            SiteDescriptor smallerDescriptor = new SiteDescriptor()
            {
                Name = CurrentDescriptor.Name,
                Proxy = CurrentDescriptor.Proxy
            };
            App.Navigate(typeof(SearchPage), new Tuple<SiteDescriptor, string>(smallerDescriptor, args.QueryText));
        }

        private void SuggestionClicked(SearchBoxResultSuggestionChosenEventArgs args)
        {
            AnySiteImage image = JsonConvert.DeserializeObject<AnySiteImage>(args.Tag);
            if (image != null)
            {
                // Minimize serialized data on suspend
                SiteDescriptor smallerDescriptor = new SiteDescriptor()
                {
                    Name = CurrentDescriptor.Name,
                    Proxy = CurrentDescriptor.Proxy
                };
                App.Navigate(typeof(ImagePage), new Tuple<SiteDescriptor, IBooruImage>(smallerDescriptor, image));
            }
        }

        private async void SuggestionsRequested(SearchBoxSuggestionsRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            if (!args.Request.IsCanceled && !string.IsNullOrEmpty(args.QueryText))
            {
                string QuickResultsString = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("QuickResultsString");
                string QuickResultString = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("QuickResultString");

                _lastSearchString = args.QueryText;

                args.Request.SearchSuggestionCollection.AppendQuerySuggestions(await CurrentDescriptor.Proxy.GetSearchSuggestions(args.QueryText));

                args.Request.SearchSuggestionCollection.AppendSearchSeparator(QuickResultsString);

                try
                {
                    List<IBooruImage> resultSuggestions = await CurrentDescriptor.Proxy.Search(2, 1, args.QueryText);
                    foreach (IBooruImage image in resultSuggestions)
                    {
                        Uri smallImageUri = new Uri(image.SmallURL);
                        RandomAccessStreamReference imageSource = RandomAccessStreamReference.CreateFromUri(smallImageUri);
                        JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
                        {
                            ContractResolver = new IgnoreAttributesContractResolver()
                        };
                        string imageJSON = JsonConvert.SerializeObject(image, jsonSettings);
                        args.Request.SearchSuggestionCollection.AppendResultSuggestion(QuickResultString, QuickResultString, imageJSON, imageSource, QuickResultString);
                    }
                }
                catch { }
            }

            deferral.Complete();
        }
    }
}
