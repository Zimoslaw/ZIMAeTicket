namespace ZIMAeTicket.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TicketsCountLabel.Text = Preferences.Get("tickets_count", "0");

            DateTime lastDBSyncDateTime = Preferences.Get("last_db_sync", DateTime.MinValue);
            if (lastDBSyncDateTime != DateTime.MinValue)
                LastDBSyncLabel.Text = lastDBSyncDateTime.ToString();
            else
                LastDBSyncLabel.Text = "Nigdy";
        }

        private async void GotoScanQR(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ScanQR");
        }

        private void CounterBtn_Clicked(object sender, EventArgs e)
        {

        }
    }
}
