using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public DateTime queryDate;

        TicketService ticketService;

        public SettingsViewModel(TicketService ticketService)
        {
            Title = "Opcje";
            this.ticketService = ticketService;
        }

        [RelayCommand]
        async Task GoToNewTicketGroup()
        {
            await Shell.Current.GoToAsync("NewTicketGroup");
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
