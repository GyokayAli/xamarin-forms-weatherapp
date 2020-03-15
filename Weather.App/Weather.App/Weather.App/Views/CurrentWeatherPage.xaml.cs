using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.App.Helpers;
using Weather.App.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        private const string APP_ID = "15ad6b24d0effce625c032cff5a327d7";

        public string Location { get; set; } = "Sofia";

        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetCoordinates();
        }

        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&APPID={APP_ID}&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);

                    //descriptionTxt.Text = weatherInfo.Weather.First().Description.ToUpper();
                    //iconImg.Source = $"w{weatherInfo.Weather.First().Icon}";
                    //cityTxt.Text = weatherInfo.Name.ToUpper();

                    //temperatureTxt.Text = weatherInfo.Main.Temp.ToString("0");
                    //humidityTxt.Text = $"{weatherInfo.Main.Humidity}%";
                    //pressureTxt.Text = $"{weatherInfo.Main.Pressure} hpa";
                    //windTxt.Text = $"{weatherInfo.Wind.Speed} m/s";
                    //cloudinessTxt.Text = $"{weatherInfo.Clouds.All}%";

                    //var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.Dt);
                    //dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();

                    GetForecast();
                    GetBackground();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }

        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid={APP_ID}&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.List)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.Dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    //dayOneTxt.Text = DateTime.Parse(allList[0].Dt_txt).ToString("dddd");
                    //dateOneTxt.Text = DateTime.Parse(allList[0].Dt_txt).ToString("dd MMM");
                    //iconOneImg.Source = $"w{allList[0].Weather[0].Icon}";
                    //tempOneTxt.Text = allList[0].Main.Temp.ToString("0");

                    //dayTwoTxt.Text = DateTime.Parse(allList[1].Dt_txt).ToString("dddd");
                    //dateTwoTxt.Text = DateTime.Parse(allList[1].Dt_txt).ToString("dd MMM");
                    //iconTwoImg.Source = $"w{allList[1].Weather[0].Icon}";
                    //tempTwoTxt.Text = allList[1].Main.Temp.ToString("0");

                    //dayThreeTxt.Text = DateTime.Parse(allList[2].Dt_txt).ToString("dddd");
                    //dateThreeTxt.Text = DateTime.Parse(allList[2].Dt_txt).ToString("dd MMM");
                    //iconThreeImg.Source = $"w{allList[2].Weather[0].Icon}";
                    //tempThreeTxt.Text = allList[2].Main.Temp.ToString("0");

                    //dayFourTxt.Text = DateTime.Parse(allList[3].Dt_txt).ToString("dddd");
                    //dateFourTxt.Text = DateTime.Parse(allList[3].Dt_txt).ToString("dd MMM");
                    //iconFourImg.Source = $"w{allList[3].Weather[0].Icon}";
                    //tempFourTxt.Text = allList[3].Main.Temp.ToString("0");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            WeatherForecastList.ItemsSource = WeatherData();
        }

        public List<Weather2> Weathers { get => WeatherData(); }

        private List<Weather2> WeatherData()
        {
            var tempList = new List<Weather2>();
            tempList.Add(new Weather2 { Temp = "22", Date = "Sunday 16", Icon = "cloud.png" });
            tempList.Add(new Weather2 { Temp = "21", Date = "Monday 17", Icon = "cloud.png" });
            tempList.Add(new Weather2 { Temp = "20", Date = "Tuesday 18", Icon = "cloud.png" });
            tempList.Add(new Weather2 { Temp = "12", Date = "Wednesday 19", Icon = "cloud.png" });
            tempList.Add(new Weather2 { Temp = "17", Date = "Thursday 20", Icon = "cloud.png" });
            tempList.Add(new Weather2 { Temp = "20", Date = "Friday 21", Icon = "cloud.png" });

            return tempList;
        }

        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Location = await GetCity(location);
                    GetWeatherInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                    return $"{placemark.Locality},{placemark.CountryCode}";
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
            }
            return Location;
        }

        private async void GetBackground()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=15&page=1";
            var pexelsAuthId = "563492ad6f917000010000011dacb69aaca240b9bf9c2a49862643c0";

            var result = await ApiCaller.Get(url, pexelsAuthId);

            if (result.Successful)
            {
                var bgInfo = JsonConvert.DeserializeObject<BackgroundInfo>(result.Response);

                if (bgInfo != null && bgInfo.Photos.Length > 0)
                {
                    var randomIndex = new Random().Next(0, bgInfo.Photos.Length - 1);
                    //bgImg.Source = ImageSource.FromUri(
                    //    new Uri(bgInfo.Photos[randomIndex].Src.Medium)
                    //    );

                }
            }
        }
    }

    public class Weather2
    {
        public string Temp { get; set; }
        public string Date { get; set; }
        public string Icon { get; set; }
    }
}
