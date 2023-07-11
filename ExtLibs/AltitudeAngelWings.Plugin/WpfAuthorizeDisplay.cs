﻿using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltitudeAngelWings.ApiClient.Client;
using AltitudeAngelWings.Plugin.Properties;

namespace AltitudeAngelWings.Plugin
{
    /// <summary>
    ///     Provides auth code URIs from an auth URI.
    /// </summary>
    public class WpfAuthorizeDisplay : IAuthorizeCodeProvider
    {
        private readonly IWin32Window _owner;
        private readonly int _width;
        private readonly int _height;
        private Uri _result;
        private Action _close = () => { };

        public WpfAuthorizeDisplay(IWin32Window owner = null, int width = 800, int height = 600)
        {
            _owner = owner;
            _width = width;
            _height = height;
        }

        public async Task<Uri> GetCodeUri(Uri authorizeUri, Uri redirectUri)
        {
            var ss = new SemaphoreSlim(0, 1);

            // BeginInvoke onto the ui thread, and await until closed
            AltitudeAngelPlugin.Instance.Host.MainForm.BeginInvoke((Action)delegate
            {
                var form = new Form();
                _result = redirectUri;
                _close = () => { form.Close(); };
                form.StartPosition = FormStartPosition.CenterParent;
                form.Width = _width;
                form.Height = _height;
                form.MaximizeBox = false;
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                form.Text = Resources.LoginWindowTitleText;
                form.Icon = Resources.AAIcon;
                var webBrowser = new WebBrowser();
                webBrowser.ScriptErrorsSuppressed = true;
                webBrowser.Navigating += WebBrowserOnNavigating;
                webBrowser.Navigated += WebBrowserOnNavigated;
                webBrowser.Dock = DockStyle.Fill;
                form.Controls.Add(webBrowser);
                webBrowser.Navigate(authorizeUri);
                form.FormClosed += (s, e) => { ss.Release(); };

                form.Show(_owner);
            });

            await ss.WaitAsync();

            return _result;
        }

        private void WebBrowserOnNavigating(object sender, WebBrowserNavigatingEventArgs navigatingEventArgs)
        {
            if (!IsRedirectUrl(navigatingEventArgs.Url)) return;
            navigatingEventArgs.Cancel = true;
            _result = navigatingEventArgs.Url;
            _close();
        }

        private void WebBrowserOnNavigated(object sender, WebBrowserNavigatedEventArgs navigatedEventArgs)
        {
            if (!IsRedirectUrl(navigatedEventArgs.Url)) return;
            _result = navigatedEventArgs.Url;
            _close();
        }

        private bool IsRedirectUrl(Uri uri)
            => uri.ToString().StartsWith(_result.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
