using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Maui.Networking;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            // Verificação de conexão com a internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Sem conexão",
                    "Verifique sua conexão com a internet e tente novamente.", "OK");
                return null;
            }

            Tempo? t = null;
            string chave = "6135072afe7f6cec1537d5cb08a5a1a2";
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    // Tratamento para cidade não encontrada
                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        await Shell.Current.DisplayAlert("Cidade não encontrada",
                            "Verifique o nome da cidade e tente novamente.", "OK");
                        return null;
                    }

                    // Tratamento para outros erros HTTP
                    if (!resp.IsSuccessStatusCode)
                    {
                        await Shell.Current.DisplayAlert("Erro na requisição",
                            $"O servidor retornou um erro: {resp.StatusCode}", "OK");
                        return null;
                    }

                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro inesperado",
                    $"Ocorreu um erro: {ex.Message}", "OK");
                return null;
            }

            return t;
        }
    }
}
