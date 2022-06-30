using BooruBrowser.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.MVVM
{
    // Base class of ViewModels
    public abstract class ViewModelBase : BindableBase
    {
        public virtual Task OnNavigatedTo(NavigationEventArgs args, object parameter, PageState pageState)
        {
            return Task.FromResult<object>(null);   // NOP
        }

        public virtual Task OnNavigatedFrom(NavigationEventArgs args, PageState pageState)
        {
            return Task.FromResult<object>(null);   // NOP
        }
    }
}
