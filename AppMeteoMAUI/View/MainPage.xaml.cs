﻿namespace AppMeteoMAUI.View;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
    }
}
