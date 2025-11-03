using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public DateTime queryDate;

        TicketService ticketService;

        SoteshopService soteshopService;

        public SettingsViewModel(TicketService ticketService, SoteshopService soteshopService)
        {
            Title = "Opcje";
            this.ticketService = ticketService;
            this.soteshopService = soteshopService;
        }

        [RelayCommand]
        async Task GoToNewTicketGroup()
        {
            await Shell.Current.GoToAsync("NewTicketGroup");
        }

        [RelayCommand]
        async Task GetTicketsFromShop()
        {

            var ticketsCount = await ticketService.CountTickets();
            Preferences.Set("tickets_count", ticketsCount.ToString());
            Preferences.Default.Set("last_db_sync", DateTime.Now);
        }

        [RelayCommand]
        async Task SyncTicketsDatbase()
        {

            var ticketsCount = await ticketService.CountTickets();
            Preferences.Set("tickets_count", ticketsCount.ToString());
            Preferences.Default.Set("last_db_sync", DateTime.Now);
        }

        // RESETOWANIE APLIKACJI
        [RelayCommand]
        async Task ResetApplication()
        {
            var confirmReset = await Shell.Current.DisplayAlert("UWAGA", "Czy na pewno chcesz usunąć WSZYSTKIE dane aplikacji i zresetować PIN?", "TAK", "NIE");

            if (!confirmReset)
                return;

            try
            {
                var clearTicketGroupResult = await ticketService.ClearTicketGroupTable();
                var clearTicketsResult = await ticketService.ClearTicketsTable();

                await Shell.Current.DisplayAlert("Resetowanie", $"Usunięto grup: {clearTicketGroupResult}\nUsunięto biletów: {clearTicketsResult}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to clear table(s): {ex}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się wyczyścić bazy danych", "OK");
            }
            
        }
    }
}
