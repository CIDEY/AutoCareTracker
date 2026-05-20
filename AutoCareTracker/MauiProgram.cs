using AutoCareTracker.Services;
using AutoCareTracker.ViewModels;
using Microsoft.Extensions.Logging;

namespace AutoCareTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<Views.AddRecordPage>();
            builder.Services.AddTransient<ViewModels.AddRecordViewModel>();
            return builder.Build();
        }
    }
}
