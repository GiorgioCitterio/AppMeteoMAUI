﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppMeteoMAUI.Model;
using System.Net.Http.Json;
using System.Web;
using AppMeteoMAUI.View;

namespace AppMeteoMAUI.ViewModel
{
    public partial class SearchViewModel : ObservableObject
    {
        private string text;
        static HttpClient client = new HttpClient();

        [ObservableProperty]
        List<Result> geocodings;
        public SearchViewModel() 
        {
            geocodings = new();
        }

        [RelayCommand]
        private async Task BackToMainPage()
        {
            await Shell.Current.GoToAsync(nameof(MainPage), false);
        }
        [RelayCommand]
        private async Task GoToForecast(Result result)
        {
            Preferences.Set("citta_scelta_search", result.Name);
            Preferences.Set("esegui_predefinito", false);
            await BackToMainPage();
        }


        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
                if (text.Length >= 2)
                {
                    SearchCity();
                }
            }
        }

        private async Task SearchCity()
        {
            Geocodings = await GeoCod(text);
        }

        static async Task<List<Result>> GeoCod(string city)
        {
            List<Result> list = new();
            try
            {
                string? cityUrlEncoded = HttpUtility.UrlEncode(city);
                string url = $"https://geocoding-api.open-meteo.com/v1/search?name={cityUrlEncoded}&language=it&count=9";
                HttpResponseMessage responseGeocoding = await client.GetAsync($"{url}");
                if (responseGeocoding.IsSuccessStatusCode)
                {
                    GeoCoding? geocodingResult = await responseGeocoding.Content.ReadFromJsonAsync<GeoCoding>();
                    if (geocodingResult != null)
                    {
                        var geo = geocodingResult.Results;
                        for (int i = 0; i < geocodingResult.Results.Count; i++)
                        {
                            list.Add(new()
                            {
                                Name = geo[i].Name,
                                CountryCode = geo[i].CountryCode,
                                Latitude = geo[i].Latitude,
                                Longitude = geo[i].Longitude,
                                Elevation = geo[i].Elevation
                            });
                        }
                        return list;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Errore!", ex.Message, "cancel");
                return null;
            }
        }
    }
}
