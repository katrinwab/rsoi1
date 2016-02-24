using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using Newtonsoft.Json.Linq;

namespace labRSOI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            webBrowser.MessageHook += webBrowser_MessageHook;
        }

        IntPtr webBrowser_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //msg = 130 is the last call for when the window gets closed on a window.close() in javascript
            if (msg == 130)
            {
                this.Close();
            }
            return IntPtr.Zero;
        }

        private void Button_Click_OAuth(object sender, RoutedEventArgs e)
        {
            var data = new StringBuilder();
            data.Append("response_type=" + Uri.EscapeDataString("code"));
            data.Append("&redirect_uri=" + Uri.EscapeDataString(RedirectUri));
            data.Append("&client_id=" + Uri.EscapeDataString(ClientId));
            data.Append("&scope=" + Uri.EscapeDataString("read_public, write_public"));
            data.Append("&state=" + Uri.EscapeDataString("768uyFys"));
            
            webBrowser.Navigate(MainUrl+OauthUrl+data);
        }

        private void OAuth(string code)
        {
            var data = new StringBuilder();
            data.Append("grant_type=" + Uri.EscapeDataString("authorization_code"));
            data.Append("&client_id=" + Uri.EscapeDataString(ClientId));
            data.Append("&client_secret=" +
                        Uri.EscapeDataString("8f9aa54c392d6e2cb46506521509af7b953055c243cf84817b3811830d144fcc"));
            data.Append("&code=" + Uri.EscapeDataString(code));

            var returnVal = DoRequest(MainUrl + AccessTokenUrl + data, PostStr);
            TextBlock.Text += returnVal + "\n";

            var o = JObject.Parse(returnVal);
            _accessToken = o["access_token"].ToString();
        }

        private void Button_Click_Anonim(object sender, RoutedEventArgs e)
        {
            var strRequest = string.Format(MainUrl +PinDetailsUrl, _accessToken);
            var returnVal = DoRequest(strRequest, GetStr);
            TextBlock.Text += returnVal + "\n";

            var o = JObject.Parse(returnVal);
            webBrowser.Navigate(o["data"]["url"].ToString());
        }

        private string DoRequest(string strRequest, string typeRequest)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(strRequest);
                request.Method = typeRequest;

                var response = request.GetResponse();
                var dataStream = response.GetResponseStream();

                if (dataStream != null)
                {
                    var responseReader = new StreamReader(dataStream);
                    var returnVal = responseReader.ReadToEnd();
                    responseReader.Close();
                    dataStream.Close();
                    response.Close();
                    return returnVal;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\n");
            }
            return "";
        }

        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().Contains("&code"))
            {
                var query = e.Uri.Query;
                OAuth(query.Substring(query.IndexOf("&code") + 6));
            }
        }

        private const string MainUrl = "https://api.pinterest.com/";
        private const string PinDetailsUrl = "v1/pins/464293042808480392/?access_token={0}";
        private const string OauthUrl = "oauth/?";
        private const string AccessTokenUrl = "v1/oauth/token?";
        private const string ClientId = "4802970826135581941";
        private const string RedirectUri = "https://mywebsite.com/connect/pinterest/";

        private const string GetStr = "GET";
        private const string PostStr = "POST";
        private string _accessToken = "";

        
    }
 
}
