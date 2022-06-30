using BooruBrowser.MVVM;
using BooruBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.ViewModels
{
    public partial class ImagePageViewModel
    {
        public ICommand SwitchToSettigsCommand { get; private set; }
        public ICommand ActivateSharingCommand { get; private set; }

        partial void PartialConstructor()
        {
            SwitchToSettigsCommand = new DelegateCommand(SwitchToSettings);
            ActivateSharingCommand = new DelegateCommand(ActivateSharing);
        }

        private void ActivateSharing()
        {
            DataTransferManager.ShowShareUI();
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

            if (App.CanGoBack())
            {
                App.GoBack();
            }
            else
            {
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
}
