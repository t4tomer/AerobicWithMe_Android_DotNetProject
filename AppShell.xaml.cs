using AerobicWithMe.Views;

namespace AerobicWithMe;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("chooseMapFromList", typeof(MapPage));



    }
}

