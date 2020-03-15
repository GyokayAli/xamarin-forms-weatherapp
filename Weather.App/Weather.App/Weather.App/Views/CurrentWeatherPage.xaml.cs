using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Location { get; set; } = "Amsterdam";

        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetCoordinates();

            // On pull to refresh
            WeatherForecastList.RefreshCommand = new Command(() =>
            {
                GetForecast();
                WeatherForecastList.IsRefreshing = false;
            });
        }

        /// <summary>
        /// Get weather information of given location.
        /// </summary>
        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&APPID={APP_ID}&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);

                    descriptionTxt.Text = weatherInfo.Weather.First().Description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.Weather.First().Icon}white";
                    cityTxt.Text = weatherInfo.Name.ToUpper();
                    temperatureTxt.Text = weatherInfo.Main.Temp.ToString("0");

                    humidityTxt.Text = $"{weatherInfo.Main.Humidity}%";
                    pressureTxt.Text = $"{weatherInfo.Main.Pressure} hpa";
                    windTxt.Text = $"{weatherInfo.Wind.Speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.Clouds.All}%";

                    var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.Dt);
                    dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();

                    GetForecast();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    await DisplayAlert("Weather Info", "No weather information found!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found!", "OK");
            }
        }

        /// <summary>
        /// Get forecast for upcoming period.
        /// </summary>
        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid={APP_ID}&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);
                    
                    var forecastList = new List<ForecastViewModel>();
                    foreach (var item in forcastInfo.List)
                    {
                        var date = DateTime.Parse(item.Dt_txt);
                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                        {
                            forecastList.Add(new ForecastViewModel
                            {
                                DateString = DateTime.Parse(item.Dt_txt).ToString("dd MMM"),
                                Icon = $"w{item.Weather[0].Icon}",
                                Temperature = item.Main.Temp.ToString("0")
                            });
                        }
                    }
                    WeatherForecastList.ItemsSource = forecastList;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    await DisplayAlert("Weather Info", "No forecast information found!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found!", "OK");
            }
        }

        /// <summary>
        /// Find current location coordinates.
        /// </summary>
        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location == null)
                {
                    throw new ArgumentNullException(nameof(location), "Location could not be retrieved from the geolocation request.");
                }

                Location = await GetCity(location);
                GetWeatherInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                await DisplayAlert("Weather Info", "Coordinates could not be found. Please enable location services on your device!", "OK");
            }
        }

        /// <summary>
        /// Find out current city from location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
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
                Console.WriteLine(fnsEx.StackTrace);
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
                Console.WriteLine(ex.StackTrace);
            }
            return Location;
        }
    }
}
