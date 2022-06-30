using BooruBrowser.Lifecycle;
using BooruBrowser.Models;
using BooruBrowser.MVVM;
using BooruBrowser.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public partial class ImagePageViewModel : ViewModelBase
    {
        partial void PartialConstructor();
        partial void PartialOnNavigatedTo(NavigationEventArgs e);
        partial void PartialOnNavigatedFrom(NavigationEventArgs e);

        private SiteDescriptor _currentSiteDescriptor;
        public SiteDescriptor CurrentSiteDescriptor
        {
            get { return _currentSiteDescriptor; }
            set { SetProperty(ref _currentSiteDescriptor, value); }
        }

        private IBooruImage _currentImageData;
        public IBooruImage CurrentImageData
        {
            get { return _currentImageData; }
            set { SetProperty(ref _currentImageData, value); }
        }

        public ICommand TagClickedCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }

        public ImagePageViewModel()
        {
            PartialConstructor();

            TagClickedCommand = new DelegateCommand<ItemClickEventArgs>(SwitchToSearchPage);

            BackCommand = new DelegateCommand(
                () => App.GoBack(),
                () => App.CanGoBack()
            );
        }

        private void SwitchToSearchPage(ItemClickEventArgs e)
        {
            string clickedItem = e.ClickedItem as string;
            if (clickedItem != null)
            {
                App.Navigate(typeof(SearchPage), new Tuple<SiteDescriptor, string>(CurrentSiteDescriptor, clickedItem));
            }
        }

        private void ShareRequestHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (CurrentImageData == null)
                return;

            var deferral = e.Request.GetDeferral();

            try
            {
                e.Request.Data.Properties.Title = "Image on " + CurrentSiteDescriptor.Name;

                // Format 1: Application Link
                string AppUri = "booru://" + CurrentSiteDescriptor.Name + "/image/" + CurrentImageData.ID.ToString();
                e.Request.Data.SetApplicationLink(new Uri(AppUri));

                // Format 2: Image
                Uri LargeUri = new Uri(CurrentImageData.BigURL);
                RandomAccessStreamReference imageSource = RandomAccessStreamReference.CreateFromUri(LargeUri);
                e.Request.Data.SetBitmap(imageSource);

                // Format 3: HTML
                string ShareHTML = "<img src=\"{0}\" /><br>"
                    + "<a href=\"{1}\">{2}</a><br>"
                    + "Uploaded by {3} to {4}";
                var htmlFormat = HtmlFormatHelper.CreateHtmlFormat(
                    string.Format(ShareHTML, CurrentImageData.SmallURL, AppUri, AppUri, CurrentImageData.Uploader,
                    CurrentSiteDescriptor.Name));
                e.Request.Data.SetHtmlFormat(htmlFormat);
            }
            catch
            {
                e.Request.FailWithDisplayText("Something went wrong, please try again later!");
            }

            deferral.Complete();
        }

        public override async Task OnNavigatedTo(NavigationEventArgs args, object parameter, PageState pageState)
        {
            DataTransferManager.GetForCurrentView().DataRequested += ShareRequestHandler;

            PartialOnNavigatedTo(args);

            BackCommand.RaiseCanExecuteChanged();

            Tuple<SiteDescriptor, IBooruImage> everything = (Tuple<SiteDescriptor, IBooruImage>)parameter;
            CurrentSiteDescriptor = everything.Item1;
            CurrentImageData = everything.Item2;
        }

        public override Task OnNavigatedFrom(NavigationEventArgs args, PageState pageState)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= ShareRequestHandler;

            PartialOnNavigatedFrom(args);

            return base.OnNavigatedFrom(args, pageState);
        }
    }
}
