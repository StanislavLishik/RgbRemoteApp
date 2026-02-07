using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RgbRemoteApp.Services;

namespace RgbRemoteApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IEspService _espService;
    private readonly ISettingsService _settingsService;
    private string _ipAddress;
    private string _statusMessage;
    private bool _isConnected;

    public MainViewModel(IEspService espService, ISettingsService settingsService)
    {
        _espService = espService;
        _settingsService = settingsService;
        _ipAddress = _settingsService.GetEspIpAddress();
        _statusMessage = "Готов к работе";

        // Initialize commands
        SendCommandCommand = new Command<string>(async (code) => await ExecuteSendCommand(code));
        SaveIpCommand = new Command(ExecuteSaveIp);
        TestConnectionCommand = new Command(async () => await ExecuteTestConnection());
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand SendCommandCommand { get; }
    public ICommand SaveIpCommand { get; }
    public ICommand TestConnectionCommand { get; }

    private async Task ExecuteSendCommand(string code)
    {
        StatusMessage = $"Отправка {code}...";
        var success = await _espService.SendCommandAsync(code);
        
        if (success)
        {
            StatusMessage = $"✓ {code} отправлено";
            IsConnected = true;
        }
        else
        {
            StatusMessage = $"✗ Ошибка отправки {code}";
            IsConnected = false;
        }
    }

    private void ExecuteSaveIp()
    {
        _settingsService.SetEspIpAddress(IpAddress);
        StatusMessage = "IP адрес сохранён";
    }

    private async Task ExecuteTestConnection()
    {
        StatusMessage = "Проверка соединения...";
        var connected = await _espService.TestConnectionAsync();
        
        IsConnected = connected;
        StatusMessage = connected ? "✓ Подключено" : "✗ Нет соединения";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
