namespace ZIMAeTicket.View;

using System.Text.RegularExpressions;
using ZIMAeTicket.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

public partial class ScanQR : ContentPage
{
    TicketService ticketService;

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
			return;
		}

        // Checking if QR code value is in correct format
        if (!Regex.IsMatch(result.Value, @"\A[0-9A-Fa-f]{64}\z"))
        {
            Shell.Current.DisplayAlert("Skanowanie biletu", "Zeskanowany kod nie jest prawidłowym biletem", "OK");
            return;
        }

        Ticket ticketFromDB = ticketService.GetTicketByHash(result.Value).Result;

        if (ticketFromDB == null)
        {
            Shell.Current.DisplayAlert("Skanowanie biletu", "Brak biletu w bazie danych", "OK");
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