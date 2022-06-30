using BooruBrowser.MVVM;
using BooruBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public partial class MainPageViewModel
    {
        public ICommand SwitchToSettigsCommand { get; private set; }

        partial void PartialConstructor()
        {
            SwitchToSettigsCommand = new DelegateCommand(SwitchToSettings);
        }

        private void SwitchToSettings()
        {
            App.Navigate(typeof(SettingsPage), null);
        }

        partial void PartialOnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtonsOnBackPressed;
        }

        partial void PartialOnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtonsOnBackPressed;
        }

        private async void HardwareButtonsOnBackPressed(object sender, BackPressedEventArgs args)
        {
            args.Handled = true;

            string ExitMessageDialogQuestion = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ExitMessageDialogQuestion");
            string ExitMessageDialogTitle = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ExitMessageDialogTitle");
            string ExitMessageDialogExit = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ExitMessageDialogExit");
            string ExitMessageDIalogCancel = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString("ExitMessageDIalogCancel");

            var dialog = new MessageDialog(ExitMessageDialogQuestion, ExitMessageDialogTitle);
            dialog.Commands.Add(new UICommand(ExitMessageDialogExit, command => Application.Current.Exit()));
            dialog.Commands.Add(new UICommand(ExitMessageDIalogCancel, command => { }));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            await dialog.ShowAsync();
        }
    }
}
