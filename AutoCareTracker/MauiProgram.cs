using AutoCareTracker.Services;
using AutoCareTracker.ViewModels;
using AutoCareTracker.Views;
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


            // Сервисы
            builder.Services.AddSingleton<DatabaseService>();

            // Гараж
            builder.Services.AddSingleton<ViewModels.GarageViewModel>();
            builder.Services.AddSingleton<Views.GaragePage>();

            // Главная
            builder.Services.AddSingleton<ViewModels.MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            // Добавление
            builder.Services.AddTransient<Views.AddRecordPage>();
            builder.Services.AddTransient<ViewModels.AddRecordViewModel>();

            return builder.Build();
        }
    }
}
