using BooruBrowser.Extensions;
using BooruBrowser.Lifecycle;
using BooruBrowser.Models;
using BooruBrowser.MVVM;
using BooruBrowser.Services;
using BooruBrowser.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        partial void PartialConstructor();
        partial void PartialOnNavigatedTo(NavigationEventArgs e);
        partial void PartialOnNavigatedFrom(NavigationEventArgs e);

        private string _lastSearchString;

        private string _mainPageTitle;
        public string MainPageTitle
        {
            get { return _mainPageTitle; }
            set { SetProperty(ref _mainPageTitle, value); }
        }

        private ObservableCollection<SiteDescriptor> _siteCollection;
        public ObservableCollection<SiteDescriptor> SiteCollection
        {
            get { return _siteCollection; }
            set { SetProperty(ref _siteCollection, value); }
        }

        private SiteDescriptor _currentDescriptor;
        public SiteDescriptor CurrentDescriptor
        {
            get { return _currentDescriptor; }
            set { SetProperty(ref _currentDescriptor, value); }
        }

        private IncrementalObservableCollection _currentItemCollection;
        public IncrementalObservableCollection CurrentItemCollection
        {
            get { return _currentItemCollection; }
            set { SetProperty(ref _currentItemCollection, value); }
        }

        private string _defaultSearchString;
        public string DefaultSearchString
        {
            get { return _defaultSearchString; }
            set { SetProperty(ref _defaultSearchString, value); }
        }

        private bool _defaultIsZoomedInViewActive;
        public bool DefaultIsZoomedInViewActive
        {
            get { return _defaultIsZoomedInViewActive; }
            set { SetProperty(ref _defaultIsZoomedInViewActive, value); }
        }

        public ICommand ViewChangeStartedCommand { get; private set; }
        public ICommand GalleryItemClickedCommand { get; private set; }
        public ICommand SuggestionsRequestedCommand { get; private set; }
        public ICommand ResultSuggestionChosenCommand { get; private set; }
        public ICommand QuerySubmittedCommand { get; private set; }
        public ICommand SiteClickedCommand { get; private set; }

        public MainPageViewModel()
        {
            PartialConstructor();

            ViewChangeStartedCommand = new DelegateCommand<SemanticZoomViewChangedEventArgs>(SwitchToSite);
            GalleryItemClickedCommand = new DelegateCommand<ItemClickEventArgs>(SwitchToItemPage);
            SiteClickedCommand = new DelegateCommand<ItemClickEventArgs>(SwitchToSitePage);
        }

        private void SwitchToSitePage(ItemClickEventArgs e)
        {
            SiteDescriptor clickedItem = e.ClickedItem as SiteDescriptor;
            if (clickedItem != null)
            {
                // Minimize serialized data on suspend
                SiteDescriptor smallerDescriptor = new SiteDescriptor()
                {
                    Name = clickedItem.Name,
                    Proxy = clickedItem.Proxy
                };
                App.Navigate(typeof(SearchPage), new Tuple<SiteDescriptor, string>(smallerDescriptor, ""));
            }
        }

        private void SwitchToItemPage(ItemClickEventArgs e)
        {
            IBooruImage clickedItem = e.ClickedItem as IBooruImage;
            if (clickedItem != null)
            {
                // Minimize serialized data on suspend
                SiteDescriptor smallerDescriptor = new SiteDescriptor()
                {
                    Name = CurrentDescriptor.Name,
                    Proxy = CurrentDescriptor.Proxy
                };
                App.Navigate(typeof(ImagePage), new Tuple<SiteDescriptor, IBooruImage>(smallerDescriptor, clickedItem));
            }
        }

        private void SwitchToSite(SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView == false)
            {
                SiteDescriptor descriptor = e.SourceItem.Item as SiteDescriptor;
                if (descriptor != null)
                {
                    CurrentDescriptor = descriptor;
                    MainPageTitle = descriptor.Name;
                    CurrentItemCollection = descriptor.NewestItems;
                }
            }
            else
            {
                string SitesString = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("SitesString");
                MainPageTitle = SitesString;
            }
        }

        public override async Task OnNavigatedTo(NavigationEventArgs args, object parameter, PageState pageState)
        {
            if (pageState.HasSavedState && pageState.Age < TimeSpan.FromMinutes(10)
                && pageState.CustomState.ContainsKey("_lastSearchString"))
            {
                _lastSearchString = (string)pageState.CustomState["_lastSearchString"];
                DefaultSearchString = _lastSearchString;
            }
            else
            {
                _lastSearchString = "";
            }

            if (pageState.HasSavedState && pageState.Age < TimeSpan.FromMinutes(10)
                && pageState.CustomState.ContainsKey("SiteCollection"))
            {
                SiteCollection = (ObservableCollection<SiteDescriptor>)pageState.CustomState["SiteCollection"];
            }
            else
            {
                SiteCollection = new ObservableCollection<SiteDescriptor>
                {
                    new SiteDescriptor()
                    {
                        Name = "Konachan",
                        Description = "Only wallpapers, tight quality control",
                        Logo = "ms-appx:///Assets/konachan_logo.png",
                        Proxy = new KonachanProxy(),
                        NewestItems = new IncrementalObservableCollection(new KonachanProxy(), null)
                    },
                    new SiteDescriptor()
                    {
                        Name = "Danbooru",
                        Description = "Everything in every resolution",
                        Logo = "ms-appx:///Assets/danbooru_logo.png",
                        Proxy = new DanbooruProxy(),
                        NewestItems = new IncrementalObservableCollection(new DanbooruProxy(), null)
                    }
                };
            }

            if (pageState.HasSavedState && pageState.Age < TimeSpan.FromMinutes(10)
                && pageState.CustomState.ContainsKey("_currentDescriptorName"))
            {
                string _currentDescriptorName = (string)pageState.CustomState["_currentDescriptorName"];
                foreach (SiteDescriptor item in SiteCollection)
                {
                    if (item.Name.Equals(_currentDescriptorName))
                    {
                        MainPageTitle = item.Name;
                        DefaultIsZoomedInViewActive = true;
                        CurrentDescriptor = item;
                        CurrentItemCollection = item.NewestItems;

                        break;
                    }
                }
            }
            else
            {
                string SitesString = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("SitesString");
                MainPageTitle = SitesString;
                DefaultIsZoomedInViewActive = false;
                CurrentDescriptor = null;
            }

            PartialOnNavigatedTo(args);
        }

        public override Task OnNavigatedFrom(NavigationEventArgs args, PageState pageState)
        {
            pageState.CustomState.Add("_lastSearchString", _lastSearchString);
            pageState.CustomState.Add("SiteCollection", SiteCollection);
            if (CurrentDescriptor != null)
                pageState.CustomState.Add("_currentDescriptorName", CurrentDescriptor.Name);

            PartialOnNavigatedFrom(args);

            return base.OnNavigatedFrom(args, pageState);
        }
    }
}
