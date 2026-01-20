using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using PeopleCounterDesktop.Converters;
using PeopleCounterDesktop.Services;
using PeopleCounterDesktop.ViewModels;
using PeopleCounterDesktop.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace PeopleCounterDesktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            _ = builder
            .UseSkiaSharp()
            .UseLiveCharts();



            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-ExtraBold.ttf", "PoppinsExtraBold");
                    fonts.AddFont("Poppins-Light.ttf", "PoppinsLight");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                });

            builder.Logging.AddDebug();

            builder.Services.AddTransient<AppShell>(); 
            builder.Services.AddSingleton<InvertedBoolConverter>();

            builder.Services.AddTransient<BuildingsPage>();
            builder.Services.AddTransient<BuildingSensorsPage>();
            builder.Services.AddTransient<ReportsPage>();

            builder.Services.AddSingleton<PeopleCounterApiService>();
            builder.Services.AddSingleton<PeopleCounterSignalRService>();

            builder.Services.AddTransient<BuildingsViewModel>();
            builder.Services.AddTransient<BuildingSensorsViewModel>();
            builder.Services.AddTransient<ReportViewModel>();
            builder.Services.AddTransient<AppShellViewModel>();


            return builder.Build();
        }
    }
}
