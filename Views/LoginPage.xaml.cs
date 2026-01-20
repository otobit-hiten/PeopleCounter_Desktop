using Microsoft.Maui.Controls;
using PeopleCounterDesktop.ViewModels;
using System;

namespace PeopleCounterDesktop.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}