using BooruBrowser.Extensions;
using BooruBrowser.Lifecycle;
using BooruBrowser.Models;
using BooruBrowser.MVVM;
using BooruBrowser.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public partial class SearchPageViewModel : ViewModelBase
    {
        partial void PartialConstructor();
        partial void PartialOnNavigatedTo(NavigationEventArgs e);
        partial void PartialOnNavigatedFrom(NavigationEventArgs e);

        private string _originalSearchString;
        private string _lastSearchString;

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

        public ICommand GalleryItemClickedCommand { get; private set; }
        public ICommand SuggestionsRequestedCommand { get; private set; }
        public ICommand ResultSuggestionChosenCommand { get; private set; }
        public ICommand QuerySubmittedCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public ICommand MobileReSearchCommand { get; private set; }

        public SearchPageViewModel()
        {
            PartialConstructor();

            GalleryItemClickedCommand = new DelegateCommand<ItemClickEventArgs>(SwitchToItemPage);

            BackCommand = new DelegateCommand(
                () => App.GoBack(),
                () => App.CanGoBack()
            );

            MobileReSearchCommand = new DelegateCommand<string>(MobileReSearch);
        }

        private void MobileReSearch(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return;

            _originalSearchString = searchText;
            _lastSearchString = searchText;
            CurrentItemCollection = new IncrementalObservableCollection(CurrentDescriptor.Proxy, searchText);
        }

        private void SwitchToItemPage(ItemClickEventArgs e)
        {
            IBooruImage clickedItem = e.ClickedItem as IBooruImage;
            if (clickedItem != null)
            {
                App.Navigate(typeof(ImagePage), new Tuple<SiteDescriptor, IBooruImage>(CurrentDescriptor, clickedItem));
            }
        }

        public override async Task OnNavigatedTo(NavigationEventArgs args, object parameter, PageState pageState)
        {
            PartialOnNavigatedTo(args);

            BackCommand.RaiseCanExecuteChanged();

            Tuple<SiteDescriptor, string> everything = (Tuple<SiteDescriptor, string>)parameter;
            CurrentDescriptor = everything.Item1;
            _originalSearchString = everything.Item2;

            if (pageState.HasSavedState && pageState.Age < TimeSpan.FromMinutes(10)
                && pageState.CustomState.ContainsKey("CurrentItemCollection"))
            {
                CurrentItemCollection = (IncrementalObservableCollection)pageState.CustomState["CurrentItemCollection"];
            }
            else
            {
                CurrentItemCollection = new IncrementalObservableCollection(CurrentDescriptor.Proxy, everything.Item2);
            }

            if (pageState.HasSavedState && pageState.Age < TimeSpan.FromMinutes(10)
                && pageState.CustomState.ContainsKey("_lastSearchString"))
            {
                _lastSearchString = (string)pageState.CustomState["_lastSearchString"];
                DefaultSearchString = _lastSearchString;
            }
            else
            {
                _lastSearchString = everything.Item2;
                DefaultSearchString = everything.Item2;
            }
        }

        public override Task OnNavigatedFrom(NavigationEventArgs args, PageState pageState)
        {
            pageState.CustomState.Add("_lastSearchString", _lastSearchString);
            pageState.CustomState.Add("CurrentItemCollection", CurrentItemCollection);

            PartialOnNavigatedFrom(args);

            return base.OnNavigatedFrom(args, pageState);
        }
    }
}
