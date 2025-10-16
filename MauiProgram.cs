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

            builder.Services.AddSingleton<TicketsViewModel>();

            builder.Services.AddTransient<TicketDetailsViewModel>();

            return builder.Build();
        }
    }
}
