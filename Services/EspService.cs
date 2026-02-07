using System.Net.Http;

namespace RgbRemoteApp.Services;

public class EspService : IEspService
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settingsService;

    public EspService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<bool> SendCommandAsync(string code)
    {
        try
        {
            var ipAddress = _settingsService.GetEspIpAddress();
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }

            var url = $"http://{ipAddress}/send?code={Uri.EscapeDataString(code)}";
            var response = await _httpClient.PostAsync(url, null);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error sending command: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var ipAddress = _settingsService.GetEspIpAddress();
            if (string.IsNullOrEmpty(ipAddress))
            {
                return false;
            }

            var url = $"http://{ipAddress}/";
            var response = await _httpClient.GetAsync(url);
            
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
