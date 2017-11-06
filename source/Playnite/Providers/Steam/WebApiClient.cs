using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using System.Threading;
using CefSharp.Wpf;
using CefSharp;
using System.Windows;
using Playnite.Controls;
using System.Text.RegularExpressions;

namespace Playnite.Providers.Steam
{
    public class WebApiClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CefSharp.OffScreen.ChromiumWebBrowser browser;

        public WebApiClient()
        {
            browser = new CefSharp.OffScreen.ChromiumWebBrowser(automaticallyCreateBrowser: false);
            browser.BrowserInitialized += Browser_BrowserInitialized;
            browser.CreateBrowser(IntPtr.Zero);
        }

        private AutoResetEvent browserInitializedEvent = new AutoResetEvent(false);
        private void Browser_BrowserInitialized(object sender, EventArgs e)
        {
            browserInitializedEvent.Set();
        }

        #region GetLoginRequired
        private void getLoginRequired_StateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false && e.Browser.HasDocument)
            {
                var b = (CefSharp.OffScreen.ChromiumWebBrowser)sender;
                if (b.Address.Contains("login"))
                {
                    loginRequired = true;
                }
                else
                {
                    loginRequired = false;
                }

                loginRequiredEvent.Set();
            }
        }

        private bool loginRequired = true;
        private AutoResetEvent loginRequiredEvent = new AutoResetEvent(false);
        public bool GetLoginRequired()
        {
            if (!browser.IsBrowserInitialized)
            {
                browserInitializedEvent.WaitOne(5000);
            }

            try
            {
                browser.LoadingStateChanged += getLoginRequired_StateChanged;
                browser.Load(loginSuccessUrl);
                loginRequiredEvent.WaitOne(10000);
                return loginRequired;
            }
            finally
            {
                browser.LoadingStateChanged -= getLoginRequired_StateChanged;
            }
        }

        #endregion GetLoginRequired

        #region GetOwnedGames
        private async void getOwnedGames_StateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false && e.Browser.HasDocument)
            {
                var b = (CefSharp.OffScreen.ChromiumWebBrowser)sender;
                var source = await b.GetSourceAsync();
                foreach (var line in source.Split('\n'))
                {
                    var match = Regex.Match(line, @"rgGames\s*\=\s*(\[.*\])");
                    if (match.Success)
                    {
                        var stringList = match.Groups[1].Value;
                        gamesList = JsonConvert.DeserializeObject<List<PrivateLibraryGame>>(stringList);
                        break;
                    }
                }

                gamesGotEvent.Set();
            }
        }

        private List<PrivateLibraryGame> gamesList;
        private AutoResetEvent gamesGotEvent = new AutoResetEvent(false);
        public List<PrivateLibraryGame> GetOwnedGames()
        {
            if (!browser.IsBrowserInitialized)
            {
                browserInitializedEvent.WaitOne(5000);
            }

            try
            {
                var name = GetUserNameUrl();
                var url = string.Format(libraryUrl, name);
                browser.LoadingStateChanged += getOwnedGames_StateChanged;
                browser.Load(url);
                gamesGotEvent.WaitOne(20000);
                return gamesList;
            }
            finally
            {
                browser.LoadingStateChanged -= getOwnedGames_StateChanged;
            }
        }

        #endregion GetOwnedGames

        #region GetUserName
        private async void getUserUrl_StateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                if (e.IsLoading == false)
                {
                    var b = (CefSharp.OffScreen.ChromiumWebBrowser)sender;
                    var source = await b.GetSourceAsync();
                    var match = Regex.Match(source, @"href\=""(.*)/inventory/""");
                    if (match.Success)
                    {
                        userUrl = match.Groups[1].Value;
                    }
                    userUrlEvent.Set();
                }
            }
        }

        private string userUrl = string.Empty;
        private AutoResetEvent userUrlEvent = new AutoResetEvent(false);
        public string GetUserNameUrl()
        {
            if (!browser.IsBrowserInitialized)
            {
                browserInitializedEvent.WaitOne(5000);
            }

            try
            {
                userUrl = string.Empty;
                browser.LoadingStateChanged += getUserUrl_StateChanged;
                browser.Load(loginSuccessUrl);
                userUrlEvent.WaitOne(10000);
                return userUrl;
            }
            finally
            {
                browser.LoadingStateChanged -= getUserUrl_StateChanged;
            }
        }
        #endregion GetUserName


        #region Login
        private void login_StateChanged(object sender, LoadingStateChangedEventArgs e)
        {            
            if (e.IsLoading == false && e.Browser.HasDocument)
            {
                var b = (ChromiumWebBrowser)sender;
                b.Dispatcher.Invoke(() =>
                {
                    if (b.Address == loginSuccessUrl)
                    {
                        loginWindow.Dispatcher.Invoke(() =>
                        {
                            loginSuccess = true;
                            loginWindow.Close();
                        });
                    }
                    else
                    {
                        loginSuccess = false;
                    }
                });
            }
        }

        private bool loginSuccess = false;
        private string loginUrl = @"https://store.steampowered.com/account/languagepreferences";
        private string libraryUrl = @"{0}/games/?tab=all";
        private string loginSuccessUrl = @"https://store.steampowered.com/account/languagepreferences";

        LoginWindow loginWindow;
        public bool Login(Window parent = null)
        {
            loginSuccess = false;
            loginWindow = new LoginWindow()
            {
                Height = 900,
                Width = 1040
            };
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.Browser.LoadingStateChanged += login_StateChanged;
            loginWindow.Owner = parent;
            loginWindow.Browser.Address = loginUrl;
            loginWindow.ShowDialog();
            loginWindow.Browser.LoadingStateChanged -= login_StateChanged;
            return loginSuccess;
        }
        #endregion Login

        public static StoreAppDetailsResult.AppDetails GetStoreAppDetail(int appId)
        {
            var url = @"http://store.steampowered.com/api/appdetails?appids={0}";
            url = string.Format(url, appId);
            var data = Web.DownloadString(url);
            var parsedData = JsonConvert.DeserializeObject<Dictionary<string, StoreAppDetailsResult>>(data);
            var response = parsedData[appId.ToString()];

            // No store data for this appid
            if (response.success != true)
            {
                return null;
            }

            return response.data;
        }
    }
}
