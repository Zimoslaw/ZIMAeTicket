using Microsoft.Extensions.Logging;
using ZIMAeTicket.Services;

namespace ZIMAeTicket
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

            builder.Services.AddSingleton<TicketService>();

            builder.Services.AddSingleton<SoteshopService>();

            builder.Services.AddSingleton<TicketsViewModel>();

            builder.Services.AddSingleton<SettingsViewModel>();

            builder.Services.AddTransient<TicketDetailsViewModel>();

            builder.Services.AddTransient<NewTicketGroupViewModel>();

            return builder.Build();
        }
    }
}
