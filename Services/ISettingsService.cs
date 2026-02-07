namespace RgbRemoteApp.Services;

public interface ISettingsService
{
    string GetEspIpAddress();
    void SetEspIpAddress(string ipAddress);
}
