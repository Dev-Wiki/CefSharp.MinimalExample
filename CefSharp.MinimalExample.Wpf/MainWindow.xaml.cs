using System.Windows;

namespace CefSharp.MinimalExample.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Browser.FrameLoadStart += BrowserOnFrameLoadStart;
            Browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
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
    }
}
