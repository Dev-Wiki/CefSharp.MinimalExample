using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using CefSharp.Event;
using CefSharp.JavascriptBinding;
using CefSharp.Wpf;
using CefSharp.Wpf.Experimental;
using CefSharp.Wpf.Internals;

namespace CefSharp.MinimalExample.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Browser.BrowserSettings.Javascript = CefState.Enabled;
            Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            Browser.FrameLoadStart += BrowserOnFrameLoadStart;
            Browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
            Browser.JavascriptObjectRepository.Register("script", new Script(), false, BindingOptions.DefaultBinder);
            // Browser.JavascriptObjectRepository.ResolveObject += JavascriptObjectRepositoryOnResolveObject;
            Browser.WpfKeyboardHandler = new WebBrowserWpfKeyboardHandler(Browser);
        }

        private void JavascriptObjectRepositoryOnResolveObject(object sender, JavascriptBindingEventArgs e)
        {
            var repo = e.ObjectRepository;
            if (e.ObjectName == "script")
            {
                repo.NameConverter = new CamelCaseJavascriptNameConverter();
                repo.Register("script", new Script(), isAsync: true, options: BindingOptions.DefaultBinder);
            }
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
        }

        private void BrowserOnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
        }

        private void JsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            object[] args = new object[2];
            args[0] = JsArgs1Tb.Text;
            args[1] = JsArgs2Tb.Text;
            if (HasValueCb.IsChecked ?? false)
            {
                ExecuteJs(JsTb.Text, (HasArgsCb.IsChecked ?? false) ? args : null);
            }
            else
            {
                ExecuteJsNoValue(JsTb.Text, (HasArgsCb.IsChecked ?? false) ? args : null);
            }
        }

        private void ExecuteJsNoValue(string js, object[] args = null)
        {
            if (!string.IsNullOrEmpty(js))
            {
                js = args == null ? js : WebBrowserExtensions.GetScriptForJavascriptMethodWithArgs(js, args);
                Browser.ExecuteScriptAsync(js);
            }
            else
            {
                JsResultTb.Text = "js is empty!";
            }
        }

        private void ExecuteJs(string js, object[] args = null)
        {
            if (!string.IsNullOrEmpty(js))
            {
                js = args == null ? js : WebBrowserExtensions.GetScriptForJavascriptMethodWithArgs(js, args);
                Browser.EvaluateScriptAsync(js).ContinueWith(o =>
                {
                    App.RunUIThread(() =>
                    {
                        JsResultTb.Text = (o?.Result?.Success ?? false) ? o?.Result?.Result?.ToString() : o?.Result?.Message;
                    });
                });
            }
            else
            {
                JsResultTb.Text = "js is empty!";
            }
        }

        public string GetWindowName()
        {
            return "MainWindow";
        }

        public void printLog(string log)
        {
            Console.WriteLine($"printLog:{log}");
        }
    }

    public class WebBrowserWpfKeyboardHandler : WpfImeKeyboardHandler
    {
        public WebBrowserWpfKeyboardHandler(ChromiumWebBrowser owner) : base(owner)
        {
        }
        public override void HandleKeyPress(KeyEventArgs e)
        {
            base.HandleKeyPress(e);
            if (e.Key == Key.F12)
            {
                owner.ShowDevTools();
            }
        }
    }
    
    public class Script
    {
        public string GetWindowName()
        {
            return "MainWindow";
        }

        public void PrintLog(string log)
        {
            Console.WriteLine($"H5 log:{log}");
        }
    }
}
