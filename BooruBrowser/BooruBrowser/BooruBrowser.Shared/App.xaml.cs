using BooruBrowser.Lifecycle;
using BooruBrowser.Models;
using BooruBrowser.Services;
using BooruBrowser.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BooruBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        private static Frame CurrentFrame { get; set; }

        public static bool Navigate(Type sourcePageType, object parameter)
        {
            string parameterString = null;

            if (parameter != null)
                parameterString = StateSerializationHelper.GetSerializedToString(parameter);

            return CurrentFrame.Navigate(sourcePageType, parameterString);
        }

        public static void GoBack()
        {
            if (CurrentFrame != null && CurrentFrame.CanGoBack)
                CurrentFrame.GoBack();
        }

        public static bool CanGoBack()
        {
            return CurrentFrame != null && CurrentFrame.CanGoBack;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        partial void InitSettings();

        // PROTOCOL ACTIVATION
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                if (eventArgs != null)
                {
                    string siteRequested = eventArgs.Uri.Host;
                    string fullRequestPath = eventArgs.Uri.AbsolutePath.Substring(1);

                    if (fullRequestPath.IndexOf('/') == -1)
                    {
                        if (Window.Current.Content == null)
                            Application.Current.Exit();
                        return;
                    }
                    string actionRequested = fullRequestPath.Substring(0, fullRequestPath.IndexOf('/'));
                    string resourceRequested = fullRequestPath.Substring(fullRequestPath.IndexOf('/') + 1);

                    List<SiteDescriptor> KnownSites = new List<SiteDescriptor>
                    {
                        new SiteDescriptor()
                        {
                            Name = "Konachan",
                            Proxy = new KonachanProxy(),
                        },
                        new SiteDescriptor()
                        {
                            Name = "Danbooru",
                            Proxy = new DanbooruProxy(),
                        }
                    };

                    foreach (SiteDescriptor item in KnownSites)
                    {
                        if (item.Name.Equals(siteRequested, StringComparison.OrdinalIgnoreCase))
                        {
                            if (actionRequested.Equals("image", StringComparison.OrdinalIgnoreCase))
                            {
                                // IMAGE
                                IBooruImage linkedImage = null;
                                try
                                {
                                    linkedImage = await item.Proxy.GetImage(Int32.Parse(resourceRequested));
                                }
                                catch { }

                                if (linkedImage != null)
                                {
                                    if (CurrentFrame != null)
                                    {
                                        Navigate(typeof(ImagePage), new Tuple<SiteDescriptor, IBooruImage>(item, linkedImage));
                                    }
                                    else
                                    {
                                        Frame rootFrame = new Frame();
                                        SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                                        rootFrame.CacheSize = 1;


                                        Window.Current.Content = rootFrame;
                                        CurrentFrame = rootFrame;

                                        if (!Navigate(typeof(ImagePage), new Tuple<SiteDescriptor, IBooruImage>(item, linkedImage)))
                                        {
                                            Application.Current.Exit();
                                            return;
                                        }

                                        Window.Current.Activate();

                                        InitSettings();
                                    }
                                }
                                else
                                {
                                    if (Window.Current.Content == null)
                                        Application.Current.Exit();
                                    return;
                                }
                            }
                            else if (actionRequested.Equals("search", StringComparison.OrdinalIgnoreCase))
                            {
                                // SEARCH
                                if (CurrentFrame != null)
                                {
                                    Navigate(typeof(SearchPage), new Tuple<SiteDescriptor, string>(item, resourceRequested));
                                }
                                else
                                {
                                    Frame rootFrame = new Frame();
                                    SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                                    rootFrame.CacheSize = 1;


                                    Window.Current.Content = rootFrame;
                                    CurrentFrame = rootFrame;

                                    if (!Navigate(typeof(SearchPage), new Tuple<SiteDescriptor, string>(item, resourceRequested)))
                                    {
                                        Application.Current.Exit();
                                        return;
                                    }

                                    Window.Current.Activate();

                                    InitSettings();
                                }
                            }
                            else
                            {
                                if (Window.Current.Content == null)
                                    Application.Current.Exit();
                                return;
                            }

                            return;
                        }
                    }

                    if (Window.Current.Content == null)
                        Application.Current.Exit();
                    return;
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                    await SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                CurrentFrame = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            InitSettings();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}