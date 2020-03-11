using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.App.Helpers;
using Weather.App.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather.App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        private string location = "Sofia";
        private const string appId = "15ad6b24d0effce625c032cff5a327d7";

        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetWeatherInfo();
        }

        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={location}&APPID={appId}&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);

                    descriptionTxt.Text = weatherInfo.Weather.First().Description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.Weather.First().Icon}";
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
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={location}&appid={appId}&units=metric";
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

                    dayOneTxt.Text = DateTime.Parse(allList[0].Dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].Dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].Weather[0].Icon}";
                    tempOneTxt.Text = allList[0].Main.Temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].Dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].Dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].Weather[0].Icon}";
                    tempTwoTxt.Text = allList[1].Main.Temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].Dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].Dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].Weather[0].Icon}";
                    tempThreeTxt.Text = allList[2].Main.Temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].Dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].Dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].Weather[0].Icon}";
                    tempFourTxt.Text = allList[3].Main.Temp.ToString("0");
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
    }
}