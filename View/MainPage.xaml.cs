namespace ZIMAeTicket.View;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TicketsCountLabel.Text = Preferences.Default.Get("tickets_count", 0).ToString();

        DateTime lastDBSyncDateTime = Preferences.Default.Get("last_db_sync", DateTime.MinValue);
        if (lastDBSyncDateTime != DateTime.MinValue)
            LastDBSyncLabel.Text = lastDBSyncDateTime.ToString();
        else
            LastDBSyncLabel.Text = "Nigdy";
    }

    private async void GotoScanQR(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ScanQR");
    }
}
