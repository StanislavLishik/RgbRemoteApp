using System.Globalization;

namespace RgbRemoteApp.Converters;

public class BoolToLedColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isConnected)
        {
            return isConnected ? Color.FromArgb("#3CFF7A") : Color.FromArgb("#1D2D3C");
        }
        return Color.FromArgb("#1D2D3C");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
