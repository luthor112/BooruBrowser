using BooruBrowser.Lifecycle;
using BooruBrowser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private bool _isLocalizationEnabled;
        public bool IsLocalizationEnabled
        {
            get { return _isLocalizationEnabled; }
            set
            {
                if (SetProperty(ref _isLocalizationEnabled, value))
                {
                    ApplicationData.Current.LocalSettings.Values["IsLocalizationEnabled"] = _isLocalizationEnabled;

                    if (_isLocalizationEnabled == false)
                        ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                    else
                        ApplicationLanguages.PrimaryLanguageOverride = "";
                }
            }
        }

        public SettingsPageViewModel()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("IsLocalizationEnabled"))
            {
                _isLocalizationEnabled = (bool)ApplicationData.Current.LocalSettings.Values["IsLocalizationEnabled"];
            }
            else
            {
                IsLocalizationEnabled = true;
            }
        }

        //partial void PartialOnNavigatedTo(NavigationEventArgs e)
        public override async Task OnNavigatedTo(NavigationEventArgs args, object parameter, PageState pageState)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        //partial void PartialOnNavigatedFrom(NavigationEventArgs e)
        public override Task OnNavigatedFrom(NavigationEventArgs args, PageState pageState)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;

            return base.OnNavigatedFrom(args, pageState);
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs args)
        {
            args.Handled = true;

            if (App.CanGoBack())
            {
                App.GoBack();
            }
        }
    }
}
