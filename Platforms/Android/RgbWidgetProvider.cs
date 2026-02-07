using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using AndroidX.Core.App;

namespace RgbRemoteApp.Platforms.Android;

[BroadcastReceiver(Label = "RGB Remote Widget", Exported = true)]
[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
[MetaData("android.appwidget.provider", Resource = "@xml/rgb_widget_provider")]
public class RgbWidgetProvider : AppWidgetProvider
{
    private const string ACTION_ON = "com.rgbremote.app.ACTION_ON";
    private const string ACTION_OFF = "com.rgbremote.app.ACTION_OFF";
    private const string ACTION_BRIGHT_UP = "com.rgbremote.app.ACTION_BRIGHT_UP";
    private const string ACTION_BRIGHT_DOWN = "com.rgbremote.app.ACTION_BRIGHT_DOWN";

    public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
    {
        foreach (var appWidgetId in appWidgetIds)
        {
            UpdateAppWidget(context, appWidgetManager, appWidgetId);
        }
    }

    private static void UpdateAppWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
    {
        var views = new RemoteViews(context.PackageName, Resource.Layout.rgb_widget);

        // Setup button click intents
        SetupButton(context, views, Resource.Id.btnOn, ACTION_ON);
        SetupButton(context, views, Resource.Id.btnOff, ACTION_OFF);
        SetupButton(context, views, Resource.Id.btnBrightUp, ACTION_BRIGHT_UP);
        SetupButton(context, views, Resource.Id.btnBrightDown, ACTION_BRIGHT_DOWN);

        appWidgetManager.UpdateAppWidget(appWidgetId, views);
    }

    private static void SetupButton(Context context,RemoteViews views,int buttonId,string action)
        {
        var intent = new Intent(context,typeof(RgbWidgetProvider));
        intent.SetAction(action);

        // ВАЖНО: используем уникальный requestCode для каждой кнопки
        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            buttonId,  // <-- это важно! Каждая кнопка должна иметь свой ID
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        views.SetOnClickPendingIntent(buttonId,pendingIntent);
        }

    public override void OnReceive(Context context,Intent intent)
        {
        base.OnReceive(context,intent);

        var action = intent.Action;
        string? command = action switch
            {
                ACTION_ON => "ON",
                ACTION_OFF => "OFF",
                ACTION_BRIGHT_UP => "BRIGHT_UP",
                ACTION_BRIGHT_DOWN => "BRIGHT_DOWN",
                _ => null
                };

        if (string.IsNullOrEmpty(command))
            return;

        // Keep the BroadcastReceiver alive for async work
        var pendingResult = GoAsync();

        Task.Run(async () =>
        {
            try
                {
                await SendCommandToEsp(context,command);
                }
            finally
                {
                pendingResult.Finish();
                }
        });
        }


    private static async Task SendCommandToEsp(Context context,string command)
        {
        try
            {
            // Preferences API в .NET MAUI использует это имя по умолчанию
            var prefs = context.GetSharedPreferences(
                $"{context.PackageName}.microsoft.maui.essentials.preferences",
                FileCreationMode.Private);

            var ipAddress = prefs.GetString("esp_ip_address","192.168.178.51");

            if (string.IsNullOrEmpty(ipAddress))
                {
                System.Diagnostics.Debug.WriteLine("Widget: IP is empty");
                global::Android.Util.Log.Debug("RGB_WIDGET","Widget: IP is empty");



                return;
                }

            // Очистка IP
            ipAddress = ipAddress.Replace("http://","")
                                 .Replace("https://","")
                                 .Replace("/","")
                                 .Trim();

            System.Diagnostics.Debug.WriteLine($"Widget sending to IP: {ipAddress}, command: {command}");
            global::Android.Util.Log.Debug("RGB_WIDGET","Command sent: " + command);

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var url = $"http://{ipAddress}/send?code={Uri.EscapeDataString(command)}";

            System.Diagnostics.Debug.WriteLine($"Widget URL: {url}");
            global::Android.Util.Log.Debug("RGB_WIDGET",$"Widget URL: {url}");

            var response = await httpClient.PostAsync(url,null);

            System.Diagnostics.Debug.WriteLine($"Widget response: {response.StatusCode}");
            global::Android.Util.Log.Debug("RGB_WIDGET",$"Widget response: {response.StatusCode}");
            }
        catch (Exception ex)
            {
            System.Diagnostics.Debug.WriteLine($"Widget error: {ex.Message}");
            global::Android.Util.Log.Error("RGB_WIDGET",$"Widget error: {ex}");
            }
        }
    }
