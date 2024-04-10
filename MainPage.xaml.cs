using SkiaSharp.Extended.UI;
using Newtonsoft.Json;
using WeatherApp.Models;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        private WeatherData _weatherData;
        private HttpClient _client;
        private GPSModule _gpsmodule;



        private IDispatcherTimer _timer;



        public WeatherData WeatherData { get { return _weatherData; } set { _weatherData = value; OnPropertyChanged(); } }

        private string _country;
        public string Country { get { return _country; } set { _country = value; OnPropertyChanged(); } }

        private double _temp;
        public double Temp { get { return _temp; } set { _temp = value; OnPropertyChanged(); } }

        private double _humidity;
        public double Humidity { get { return _humidity; } set { _humidity = value; OnPropertyChanged(); } }

        private double pressure;
        public double Pressure { get { return pressure; } set { pressure = value; OnPropertyChanged(); } }

        private double wind;
        public double Wind { get { return wind; } set { wind = value; OnPropertyChanged(); } }

        private int clouds;
        public int Clouds { get { return clouds; } set { clouds = value; OnPropertyChanged(); } }


        private string _time;
        public string Time { get { return _time; } set { _time = value; OnPropertyChanged(); } }

        private string _sunset;
        public string Sunset { get { return _sunset; } set { _sunset = value; OnPropertyChanged(); } }

        private string _sunrise;
        public string sunrise { get { return _sunrise; } set { _sunrise = value; OnPropertyChanged(); } }

        private string _description;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged(); } }
        private ConvertToKilometers _convertToKilometers;
        public MainPage()
        {
            InitializeComponent();
            _gpsmodule = new GPSModule();
            _client = new HttpClient();
            _convertToKilometers = new ConvertToKilometers();
            _weatherData = new WeatherData();
            GetLatestWeather();



            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timertick;
            _timer.Start();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _timer.Start();
            GetLatestWeather();
        }

       
        
        private void Timertick(object? sender, EventArgs e)
        {
            GetLatestWeather();
            
        }

        public ICommand RefreshCommand => new Command(GetLatestWeather);
        
        public async void  GetLatestWeather()
        {
            activityindicator.IsRunning = true;
            activityindicator.IsVisible = true;
            activityindicator.HeightRequest = 30;
            activityindicator.WidthRequest = 30;
            

                    Location location = await _gpsmodule.GetCurrentLocation();
            double lat = location.Latitude;
            double lng = location.Longitude;

            string appid = "84e1ae5b22423295b04911bcbcb78422";
            string response = await _client.GetStringAsync(new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lng}&appid={appid}&units=metric"));

           WeatherData = JsonConvert.DeserializeObject<WeatherData>(response);

            
                DateTimeOffset dtoffset = DateTimeOffset.FromUnixTimeSeconds(WeatherData.dt);
                Time = dtoffset.UtcDateTime.ToString();

                 dtoffset = DateTimeOffset.FromUnixTimeSeconds(WeatherData.sys.sunrise);
                sunrise = dtoffset.UtcDateTime.ToString();

                dtoffset = DateTimeOffset.FromUnixTimeSeconds(WeatherData.sys.sunset);
                Sunset = dtoffset.UtcDateTime.ToString();
            
           Country = WeatherData.sys.country;
            Temp = Math.Round(WeatherData.main.temp);
            Humidity = WeatherData.main.humidity;
            Pressure = WeatherData.main.pressure;
            Wind = WeatherData.wind.speed;
            Clouds = WeatherData.clouds.all;
            Description = WeatherData.weather[0].description.ToUpper();
            
            activityindicator.IsRunning = false;
            activityindicator.IsVisible = false;
            
        }
    
    
        
    
    }

}
