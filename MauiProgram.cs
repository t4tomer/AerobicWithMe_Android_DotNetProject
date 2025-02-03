using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Maui.GoogleMaps.Hosting;


namespace AerobicWithMe;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()

        // this if is used for turning on the google maps . 
            #if ANDROID
			            .UseGoogleMaps()
            #elif IOS
			            //.UseGoogleMaps("AIzaSyCVDsKsFCUPb3nW9Aua2unyeETmgi6W3m8")
            #endif

            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

