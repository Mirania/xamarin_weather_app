using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class StatsPage : ContentPage
    {
        private string apikey = "bf44898d91e348588e1192019192604";
        private Entry entry;
        private XElement xml;
        private string r1;
        private string r2;
        private MainPage mainPage;

        public StatsPage(Entry e, MainPage m)
        {
            InitializeComponent();
            title.Text = "Loading...";
            entry = e;
            mainPage = m;          
            FetchFromAPI(entry.Name);
            mainPage.TouchEntry(e);
        }

        public async void FetchFromAPI(string city)
        {
            string link = "http://api.apixu.com/v1/current.xml?key="+apikey+"&q="+city+",Portugal";
            xml = XElement.Parse(await GetAsync(link));
            fillTable();
        }

        private void fillTable()
        {
            XElement l = xml.Element("location");
            XElement c = xml.Element("current");

            title.Text = l.Element("name").Value;
            img.Source = "https:" + c.Element("condition").Element("icon").Value;
            flavor.Text = c.Element("condition").Element("text").Value;

            req.Text = RelativeTime(l.Element("localtime_epoch").Value);
            col.Text = RelativeTime(c.Element("last_updated_epoch").Value);
            temp.Text = String.Format("{0} ºC / {1} ºF", c.Element("temp_c").Value, c.Element("temp_f").Value);
            prec.Text = String.Format("{0} mm. / {1} in.", c.Element("precip_mm").Value, c.Element("precip_in").Value);
            wind.Text = String.Format("{0} km/h / {1} mph ({2})", c.Element("wind_kph").Value, c.Element("wind_mph").Value, c.Element("wind_dir").Value);
            hum.Text = c.Element("humidity").Value + "%";
        }

        public void Refresh(object sender, EventArgs e)
        {
            title.Text = "Loading...";
            img.Source = "";
            flavor.Text = "";

            req.Text = "-";
            col.Text = "-";
            temp.Text = "-";
            prec.Text = "-";
            wind.Text = "-";
            hum.Text = "-";
            FetchFromAPI(entry.Name);
        }

        public void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private string RelativeTime(string unixTime)
        {
            long now = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            long diff = now - long.Parse(unixTime);

            long h = diff / (60 * 60);
            long m = (diff - 60 * 60 * h) / 60;

            if (h == 0 && m < 2) return "Just now";

            string r = "";
            if (h > 0) r += (h == 1 ? h + " hour" : h + " hours");
            if (h > 0 && m > 0) r += ", ";
            if (m > 0) r += (m == 1 ? m + " minute" : m + " minutes");
            return r + " ago";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60 * 1000; // 60 secs
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (xml == null) return;
            r1 = RelativeTime(xml.Element("location").Element("localtime_epoch").Value);
            r2 = RelativeTime(xml.Element("current").Element("last_updated_epoch").Value);

            Action updater = () => { UpdateGUI(); };
            Device.BeginInvokeOnMainThread(updater);
        }

        private void UpdateGUI()
        {
            if (r1 == null || r2 == null) return;
            req.Text = r1;
            col.Text = r2;
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}