namespace RgbRemoteApp.Services;

public interface IEspService
{
    Task<bool> SendCommandAsync(string code);
    Task<bool> TestConnectionAsync();
}
