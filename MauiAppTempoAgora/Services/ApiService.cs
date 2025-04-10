using System.Net;
using MauiAppTempoAgora.Models;
using Newtonsoft.Json;

public static class ApiService
{
    private const string API_KEY = "SUA_CHAVE_API"; // Substitua pela sua chave da OpenWeather

    public static async Task<Tempo?> ObterDadosClimaAsync(string txt_cidade)
    {
        try
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={txt_cidade}&appid={API_KEY}&units=metric&lang=pt";

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var tempo = JsonConvert.DeserializeObject<Tempo>(json);
                return tempo;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Cidade não encontrada. Verifique o nome e tente novamente.");
            }
            else
            {
                throw new Exception("Erro ao buscar os dados do clima.");
            }
        }
        catch (HttpRequestException)
        {
            throw new Exception("Sem conexão com a internet. Verifique sua rede e tente novamente.");
        }
    }
}