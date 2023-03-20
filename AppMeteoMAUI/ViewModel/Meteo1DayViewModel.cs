﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using AppMeteoMAUI.View;
using AppMeteoMAUI.Model;

namespace AppMeteoMAUI.ViewModel
{
    public partial class Meteo1DayViewModel : ObservableObject
    {
        [ObservableProperty]
        string text;
        [ObservableProperty]
        double temperatura;
        [ObservableProperty]
        string city;
        public ObservableCollection<ForecastDaily> currentForecast { get; set; }
        static HttpClient? client = new HttpClient();
        string result;
        public Meteo1DayViewModel()
        {
            currentForecast = new ObservableCollection<ForecastDaily>();
            PrendiPosizionePredefinita();
        }
        #region Posizione Predefinita
        private async void PrendiPosizionePredefinita()
        {
            string path = FileSystem.AppDataDirectory + "/UltimaPosizioneSalvata.json";
            if (!File.Exists(path))
            {
                FileStream fileStream = File.Create(path);
                var options = new JsonSerializerOptions() { WriteIndented = true };
                PosizionePredefinita posizione = new();
                do
                {
                    result = await App.Current.MainPage.DisplayPromptAsync("Inserire la posizione predefinita", "");
                } while (result == null || result.Length == 0);
                if (result != null)
                {
                    posizione.posizionePredefinita = result;
                }
                await JsonSerializer.SerializeAsync(fileStream, posizione, options);
                await fileStream.DisposeAsync();
            }
            string fileJson = File.ReadAllText(path);
            PosizionePredefinita pos = JsonSerializer.Deserialize<PosizionePredefinita>(fileJson);
            (double? lat, double? lon)? geo = await GeoCod(pos.posizionePredefinita);
            FormattableString urlAdd = $"https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&&hourly=temperature_2m,weathercode,windspeed_10m,winddirection_10m,apparent_temperature,precipitation_probability,precipitation,showers&daily=weathercode,temperature_2m_max,temperature_2m_min,sunrise,sunset,apparent_temperature_max,apparent_temperature_min&current_weather=true&timeformat=unixtime&forecast_days=1&timezone=auto";
            await StampaDatiAsync(urlAdd);
            City = pos.posizionePredefinita;
        }
        #endregion

        #region Geolocalizzazione

        [RelayCommand]
        public async Task GetCurrentLocation()
        {
            Location location = await Geolocation.Default.GetLastKnownLocationAsync();
            FormattableString urlAdd = $"https://api.open-meteo.com/v1/forecast?latitude={location.Latitude}&longitude={location.Longitude}&&hourly=temperature_2m,weathercode,windspeed_10m,winddirection_10m,apparent_temperature,precipitation_probability,precipitation,showers&daily=weathercode,temperature_2m_max,temperature_2m_min,sunrise,sunset,apparent_temperature_max,apparent_temperature_min&current_weather=true&timeformat=unixtime&forecast_days=1&timezone=auto";
            await StampaDatiAsync(urlAdd);
            FormattableString formattableString = $"https://api.bigdatacloud.net/data/reverse-geocode-client?latitude={location.Latitude}&longitude={location.Longitude}&localityLanguage=en";
            string urlRecuperaCity = FormattableString.Invariant(formattableString);
            CittàDaCoordinate cittàDaCoordinate = await client.GetFromJsonAsync<CittàDaCoordinate>(urlRecuperaCity);
            City = cittàDaCoordinate.City;
        }
        #endregion

        #region Cerca località

        [RelayCommand]
        public async Task CercaLocalita()
        {
            string city = Text;
            (double? lat, double? lon)? geo = await GeoCod(city);
            FormattableString urlAdd = $"https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&&hourly=temperature_2m,weathercode,windspeed_10m,winddirection_10m,apparent_temperature,precipitation_probability,precipitation,showers&daily=weathercode,temperature_2m_max,temperature_2m_min,sunrise,sunset,apparent_temperature_max,apparent_temperature_min&current_weather=true&timeformat=unixtime&forecast_days=1&timezone=auto";
            await StampaDatiAsync(urlAdd);
            City = city;
        }
        #endregion

        #region StampaDati
        public async Task StampaDatiAsync(FormattableString urlAddUnformattable)
        {
            string urlAdd = FormattableString.Invariant(urlAddUnformattable);
            var response = await client.GetAsync(urlAdd);
            if (response.IsSuccessStatusCode)
            {
                ForecastDaily forecastDaily = await response.Content.ReadFromJsonAsync<ForecastDaily>();
                if (forecastDaily.Daily != null)
                {
                    var fd = forecastDaily.Hourly;
                    currentForecast.Clear();
                    for (int i = 0; i < fd.Time.Count; i++)
                    {
                        (string, ImageSource) datiImmagine = WMOCodesIntIT(fd.Weathercode[i]);
                        CurrentForecast1Day objCur = new CurrentForecast1Day()
                        {
                            Temperature2m = fd.Temperature2m[i],
                            ApparentTemperature = fd.ApparentTemperature[i],
                            DescMeteo = datiImmagine.Item1,
                            ImageUrl = datiImmagine.Item2,
                            Time = UnixTimeStampToDateTime(fd.Time[i]),
                        };
                        currentForecast.Add(new ForecastDaily() { CurrentForecast1Day = objCur});
                    }
                    Temperatura = forecastDaily.CurrentWeather.Temperature;
                }
            }
        }
        #endregion

        #region Metodi Aggiungitivi
        static async Task<(double? lat, double? lon)?> GeoCod(string city)
        {
            string? cityUrlEncoded = HttpUtility.UrlEncode(city);
            string url = $"https://geocoding-api.open-meteo.com/v1/search?name={cityUrlEncoded}&language=it&count=7";
            HttpResponseMessage responseGeocoding = await client.GetAsync($"{url}");
            if (responseGeocoding.IsSuccessStatusCode)
            {
                GeoCoding? geocodingResult = await responseGeocoding.Content.ReadFromJsonAsync<GeoCoding>();
                if (geocodingResult != null)
                {
                    return (geocodingResult.Results[0].Latitude, geocodingResult.Results[0].Longitude);
                }
            }
            return null;
        }
        private static int? UnixTimeStampToDateTime(double? unixTimeStamp)
        {
            if (unixTimeStamp != null)
            {
                DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
                return dateTime.Hour;
            }
            return null;
        }
        static (string, ImageSource) WMOCodesIntIT(int? code)
        {
            return code switch
            {
                0 => ("cielo sereno", ImageSource.FromFile("clear_day.svg")),
                1 => ("limpido", ImageSource.FromFile("partly_cloudy_day.svg")),
                2 => ("annuvolato", ImageSource.FromFile("cloudy.svg")),
                3 => ("coperto", ImageSource.FromFile("extreme_rain.svg")),
                45 => ("nebbia", ImageSource.FromFile("fog.svg")),
                48 => ("brina", ImageSource.FromFile("extreme_fog.svg")),
                51 => ("pioggerella", ImageSource.FromFile("drizzle.svg")),
                53 => ("pioggerella", ImageSource.FromFile("drizzle.svg")),
                55 => ("pioggerella intensa", ImageSource.FromFile("drizzle.svg")),
                56 => ("pioggerella gelata", ImageSource.FromFile("sleet.svg")),
                57 => ("pioggerella gelata", ImageSource.FromFile("extreme_sleet.svg")),
                61 => ("pioggia scarsa", ImageSource.FromFile("drizzle.svg")),
                63 => ("pioggia moderata", ImageSource.FromFile("drizzle.svg")),
                65 => ("pioggia intensa", ImageSource.FromFile("extrene_drizzle.svg")),
                66 => ("pioggia gelata", ImageSource.FromFile("sleet.svg")),
                67 => ("pioggia gelata", ImageSource.FromFile("extreme_sleet.svg")),
                71 => ("nevicata lieve", ImageSource.FromFile("snow.svg")),
                73 => ("nevicata media", ImageSource.FromFile("snow.svg")),
                75 => ("nevicata intensa", ImageSource.FromFile("extreme_snow.svg")),
                77 => ("granelli di neve", ImageSource.FromFile("sleet.svg")),
                80 => ("pioggia debole", ImageSource.FromFile("drizzle.svg")),
                81 => ("pioggia moderata", ImageSource.FromFile("drizzle.svg")),
                82 => ("pioggia violenta", ImageSource.FromFile("extreme_drizzle.svg")),
                85 => ("neve leggera", ImageSource.FromFile("snow.svg")),
                86 => ("neve pesante", ImageSource.FromFile("extreme_snow.svg")),
                95 => ("temporale lieve", ImageSource.FromFile("drizzle.svg")),
                96 => ("temporale grandine", ImageSource.FromFile("sleet.svg")),
                99 => ("temporale grandine", ImageSource.FromFile("extreme_sleet.svg"))
            };
        }
        #endregion
    }
}

