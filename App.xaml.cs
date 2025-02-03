using AerobicWithMe.Services;

namespace AerobicWithMe;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}