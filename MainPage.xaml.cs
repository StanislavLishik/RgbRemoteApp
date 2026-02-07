using RgbRemoteApp.ViewModels;

namespace RgbRemoteApp;

public partial class MainPage : ContentPage
    {
    public MainPage(MainViewModel viewModel)
        {
        InitializeComponent();
        BindingContext = viewModel;
        }
    }
