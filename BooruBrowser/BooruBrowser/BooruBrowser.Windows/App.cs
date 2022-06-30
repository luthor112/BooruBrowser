using BooruBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;

namespace BooruBrowser
{
    public partial class App
    {
        partial void InitSettings()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
        }

        private void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("App", "BooruBrowser Settings", command => new AppSettingsFlyout().Show()));
        }
    }
}
