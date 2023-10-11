using Pi4_Patatzaak.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WelcomeMessageApiClient
{
    private readonly HttpClient _httpClient;
    private readonly DotEnvVariables _dotEnvVariables;

    public WelcomeMessageApiClient(DotEnvVariables dotEnvVariables)
    {
        _dotEnvVariables = dotEnvVariables;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_dotEnvVariables.ApiBaseUrl)
        };
    }

    public async Task<string> GetRandomWelcomeMessageAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_dotEnvVariables.ApiWelcomeMessage);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij het doen van het API-verzoek: {ex.Message}");
            return null;
        }
    }
}
