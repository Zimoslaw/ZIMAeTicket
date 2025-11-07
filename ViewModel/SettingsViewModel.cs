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
            IsBusy = true;

            try
            {
                int newTicketsCount = 0;

                var groups = await ticketService.GetAllTicketGroups();

                if (groups.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Brak grup biletów. Dodaj przynajmniej jedną grupę.", "OK");
                    return;
                }

                foreach (TicketGroup group in groups)
                {
                    List<Ticket> tickets = await soteshopService.GetTicketsFromShopByDate(group.Id, QueryDate);

                    foreach (Ticket ticket in tickets)
                    {
                        for (int i = 0; i < ticket.Quantity; i++)
                        {
                            await ticketService.AddNewTicket(ticket);
                            newTicketsCount++;
                        } 
                    }
                }

                // Stats update
                var ticketsCount = await ticketService.CountTickets();
                Preferences.Set("tickets_count", ticketsCount.ToString());
                var pendingTicketsCount = await ticketService.CountPendingTickets();
                Preferences.Set("pending_tickets", pendingTicketsCount);
                // Last sync date update
                Preferences.Default.Set("last_db_sync", DateTime.Now);

                if (newTicketsCount > 0)
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu.", "OK");
                else
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu. Status API: {soteshopService.StatusMessage}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd przy pobieraniu biletów ze sklepu: {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        async Task SyncTicketsDatabase()
        {
            IsBusy = true;

            try
            {
                int newTicketsCount = 0;

                var groups = await ticketService.GetAllTicketGroups();

                if (groups.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Synchronizacja biletów", $"Brak grup biletów. Dodaj przynajmniej jedną grupę.", "OK");
                    return;
                }

                List<Ticket> tickets = await soteshopService.GetTicketsByDate(QueryDate);

                foreach (Ticket ticket in tickets)
                {
                    await ticketService.AddNewTicket(ticket);
                    newTicketsCount++;
                }

                // Stats update
                var ticketsCount = await ticketService.CountTickets();
                Preferences.Set("tickets_count", ticketsCount.ToString());
                // Last sync date update
                Preferences.Default.Set("last_db_sync", DateTime.Now);

                if (newTicketsCount > 0)
                    await Shell.Current.DisplayAlert("Synchronizowanie bazy biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu.", "OK");
                else
                    await Shell.Current.DisplayAlert("Synchronizowanie bazy biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu. Status API: {soteshopService.StatusMessage}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd przy synchronizacji bazy biletów: {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }

        // RESETOWANIE APLIKACJI
        [RelayCommand]
        async Task ResetApplication()
        {
            var confirmReset = await Shell.Current.DisplayAlert("UWAGA", "Czy na pewno chcesz usunąć WSZYSTKIE dane aplikacji i zresetować PIN?", "TAK", "NIE");

            IsBusy = true;

            if (!confirmReset)
                return;

            try
            {
                // Clear tickets db
                var clearTicketGroupResult = await ticketService.ClearTicketGroupTable();
                var clearTicketsResult = await ticketService.ClearTicketsTable();

                // Stats update
                var ticketsCount = await ticketService.CountTickets();
                Preferences.Set("tickets_count", ticketsCount.ToString());

                await Shell.Current.DisplayAlert("Resetowanie", $"Usunięto grup: {clearTicketGroupResult}\nUsunięto biletów: {clearTicketsResult}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to clear table(s): {ex}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się wyczyścić bazy danych", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
