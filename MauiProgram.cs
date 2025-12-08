using Microsoft.Extensions.Logging;
using ZIMAeTicket.Services;
using ZXing.Net.Maui.Controls;

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
                })
                .UseBarcodeReader();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<TicketService>();
#if WINDOWS
            builder.Services.AddSingleton<MailingService>();
#endif
            builder.Services.AddSingleton<TicketsViewModel>();

            builder.Services.AddSingleton<SettingsViewModel>();
#if WINDOWS
            builder.Services.AddSingleton<MailingViewModel>();
#endif
            builder.Services.AddTransient<TicketDetailsViewModel>();

            builder.Services.AddTransient<NewTicketGroupViewModel>();

            return builder.Build();
        }
    }
}
