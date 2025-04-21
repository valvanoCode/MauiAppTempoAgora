using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Net;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verifica se há conexão com a internet
                if (!await VerificarConexaoInternet())
                {
                    await DisplayAlert("Sem conexão", "Você está sem conexão com a internet. Conecte-se para verificar o tempo.", "OK");
                    return;
                }

                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Decrição: {t.description} \n" +
                                         $"Velocidade do Vento: {t.speed} \n" +
                                         $"Visibilidade: {t.visibility} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n";

                        lbl_res.Text = dados_previsao;

                    }
                    else
                    {
                        await DisplayAlert("Erro", "Sem dados de previsão para esta cidade.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Atenção", "Por favor, preencha o nome da cidade.", "OK");
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                await DisplayAlert("Cidade não encontrada", "Não foi possível encontrar a cidade especificada. Verifique o nome e tente novamente.", "OK");
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Erro na requisição", $"Ocorreu um erro ao buscar os dados: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro inesperado", $"Ocorreu um erro: {ex.Message}", "OK");
            }
        }

        private async Task<bool> VerificarConexaoInternet()
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var resposta = await ping.SendPingAsync("8.8.8.8", 3000);
                    return resposta.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}



