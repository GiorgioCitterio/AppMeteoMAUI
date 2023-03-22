using AppMeteoMAUI.ViewModel;

namespace AppMeteoMAUI.View;

public partial class Settings : ContentPage
{
	public Settings(SettingsViewModel viewModel)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        BindingContext = viewModel;
    }
}