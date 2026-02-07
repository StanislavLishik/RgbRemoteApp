namespace RgbRemoteApp.Services;

public class SettingsService : ISettingsService
{
    private const string EspIpAddressKey = "esp_ip_address";
    private const string DefaultIpAddress = "192.168.1.100"; // Default IP

    public string GetEspIpAddress()
    {
        return Preferences.Get(EspIpAddressKey, DefaultIpAddress);
    }

    public void SetEspIpAddress(string ipAddress)
    {
        Preferences.Set(EspIpAddressKey, ipAddress);
    }
}
