using BooruBrowser.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BooruBrowser.MVVM
{
    // Base class of Pages
    public abstract class PageBase : Page
    {
        protected ViewModelBase ViewModel
        {
            get { return DataContext as ViewModelBase; }    // Cast, hiba eseten null-al ter vissza
        }

        private String _pageKey;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_pageKey != null) return;

            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            _pageKey = "Page-" + Frame.BackStackDepth;

            object parameter = null;
            if (e.Parameter != null)
            {
                parameter = StateSerializationHelper.GetDeserializedObject((string)e.Parameter);
            }

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the navigation stack
                var nextPageKey = _pageKey;
                int nextPageIndex = Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                if (ViewModel != null)
                    await ViewModel.OnNavigatedTo(e, parameter, new PageState());
            }
            else
            {
                var pageState = (PageState)frameState[_pageKey];
                await LoadState(parameter, pageState);

                if (ViewModel != null)
                    await ViewModel.OnNavigatedTo(e, parameter, pageState);
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(Frame);
            var pageState = PageState.CreateForSave(e.Parameter);
            await SaveState(pageState);

            if (ViewModel != null)
                await ViewModel.OnNavigatedFrom(e, pageState);

            frameState[_pageKey] = pageState;
        }

        protected virtual Task LoadState(Object navigationParameter, PageState pageState)
        {
            return Task.FromResult<object>(null);
        }

        protected virtual Task SaveState(PageState pageState)
        {
            return Task.FromResult<object>(null);
        }
    }
}
