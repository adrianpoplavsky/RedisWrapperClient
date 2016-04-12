using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RedisWrapperClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            txtsetContent.Text = @"{ ""id"":1,""name"":""Adrian"",""surname"":""Puka"",""address"":""bien ahi 234"",""telefono"":""3434923948""}";
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var startTime = DateTime.Now;

            try
            {
                using (var client = new HttpClient())
                {
                    var entity = txtEntity.Text;
                    var identifier = txtIdentifier.Text;

                    client.BaseAddress = new Uri(string.Format("http://rediswrapper.azurewebsites.net/get/{0}/{1}", entity, identifier));

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();

                        var objecta = JsonConvert.DeserializeObject<dynamic>(data.Result.ToString());

                        listView.ItemsSource = new List<dynamic>() { objecta };

                        tbTimeTaken.Text = "TimeTaken: " + (DateTime.Now - startTime).Milliseconds + "ms.";
                    }
                    else
                    {
                        tbTimeTaken.Text = "TimeTaken: " + (DateTime.Now - startTime).Milliseconds + "ms. Error: " + response.ReasonPhrase;
                    }
                }
            }
            catch (Exception ex)
            {
                tbTimeTaken.Text = "TimeTaken: " + (DateTime.Now - startTime).Milliseconds + "ms. Error: " + ex.Message;
                return;
            }
        }

        private async void btsetCall_Click(object sender, RoutedEventArgs e)
        {
            var entity = txtsetEntityName.Text;
            var content = txtsetContent.Text;
            var startTime = DateTime.Now;

            try
            {
                var uri = new Uri(string.Format("http://rediswrapper.azurewebsites.net/set/{0}", entity));
                var request = (HttpWebRequest)WebRequest.Create(uri);
                var postContent = Encoding.UTF8.GetBytes(content);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    requestStream.Write(postContent, 0, postContent.Length);
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync()) { }
            }
            catch (Exception ex)
            {
                tbsetTimeTaken.Text = "TimeTaken: " + (DateTime.Now - startTime).Milliseconds + "ms. Error: " + ex.Message;
                return;
            }

            tbsetTimeTaken.Text = "TimeTaken: " + (DateTime.Now - startTime).Milliseconds + "ms.";
        }
    }
}
