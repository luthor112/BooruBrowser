using BooruBrowser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Storage;

namespace BooruBrowser.ViewModels
{
    public class AppSettingsFlyoutViewModel : BindableBase
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

        public AppSettingsFlyoutViewModel()
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
    }
}
