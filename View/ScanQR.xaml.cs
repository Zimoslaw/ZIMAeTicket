namespace ZIMAeTicket.View;

using System.Text.RegularExpressions;
using ZIMAeTicket.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

public partial class ScanQR : ContentPage
{
    readonly TicketService ticketService;

    [GeneratedRegex(@"\A[0-9A-Fa-f]{64}\z")]
    private static partial Regex HashRegex();

    public ScanQR()
    {
        InitializeComponent();

        BindingContext = this;

        ticketService = new TicketService();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        PermissionStatus cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (cameraStatus != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("Skanowanie biletów", "Nie zezwolono na użycie kamery. Zmień uprawnienia aplikacji", "OK");
        }

        BarcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            TryHarder = true

        };
        BarcodeReader.CameraLocation = CameraLocation.Rear;

        BarcodeReader.BarcodesDetected += OnBarcodesDetected;
    }

    private void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        if (sender is CameraBarcodeReaderView bcr)
            bcr.BarcodesDetected -= OnBarcodesDetected;

        var result = e.Results.FirstOrDefault();

        if (result == null)
        {
            Dispatcher.DispatchAsync(async () =>
                await Shell.Current.DisplayAlert("Skanowanie biletu", "Brak prawidłowego kodu w obiektywie", "OK"));
            BarcodeReader.BarcodesDetected += OnBarcodesDetected;
            return;
        }

        // Checking if QR code value is in correct format
        if (!HashRegex().IsMatch(result.Value))
        {
            Dispatcher.DispatchAsync(async () =>
                await Shell.Current.DisplayAlert("Skanowanie biletu", "Zeskanowany kod nie jest prawidłowym biletem", "OK"));
            BarcodeReader.BarcodesDetected += OnBarcodesDetected;
            return;
        }

        Ticket ticketFromDB = ticketService.GetTicketByHash(result.Value).Result;

        if (ticketFromDB == null)
        {
            Dispatcher.DispatchAsync(async () =>
                await Shell.Current.DisplayAlert("Skanowanie biletu", "Brak biletu w bazie danych", "OK"));
            BarcodeReader.BarcodesDetected += OnBarcodesDetected;
            return;
        }

        if (string.IsNullOrEmpty(ticketFromDB.OrderId))
        {
            Dispatcher.DispatchAsync(async () =>
                await Shell.Current.DisplayAlert("Skanowanie biletu", $"Błąd podczas pobierania danych: {ticketService.StatusMessage}", "OK"));
            BarcodeReader.BarcodesDetected += OnBarcodesDetected;
            return;
        }

        Dispatcher.DispatchAsync(async () =>
        {
            await Shell.Current.GoToAsync("TicketDetails", true,
                    new Dictionary<string, object>
                    {
                    {"Ticket", ticketFromDB }
                    });
        });
    }
}