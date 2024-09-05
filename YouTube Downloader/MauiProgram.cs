using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Plugin.Maui.Audio;
using YouTube_Downloader.Controllers;
using YouTube_Downloader.ViewModels;
using YouTube_Downloader.Views;

namespace YouTube_Downloader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .AddAudio()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IQueueDownloadController, QueueDownloadController>();


#if DEBUG
            builder.Logging.AddDebug();

            var mauiApp = builder.Build();

            var serviceProvider = mauiApp.Services;
            App.Services = serviceProvider;
#endif

            return mauiApp;
        }
    }
}
