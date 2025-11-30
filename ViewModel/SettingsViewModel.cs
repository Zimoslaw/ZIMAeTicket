using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public DateTime queryDate;

        readonly TicketService ticketService;

        readonly SoteshopService soteshopService;

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
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Pobieranie biletów", $"Brak połączenia z Internetem. Połącz się z Wi-Fi lub włącz dane komórkowe", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                int newTicketsCount = 0;

                var groups = await ticketService.GetAllTicketGroups();

                if (groups.Count < 1)
                {
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Brak grup biletów. Dodaj przynajmniej jedną grupę.", "OK");
                    return;
                }

                foreach (TicketGroup group in groups)
                {
                    List<Ticket> tickets = await soteshopService.GetTicketsFromShopByDate(group.Id, QueryDate);

                    foreach (Ticket ticket in tickets)
                    {
                        // checking if tickets from that order and group were already downloaded
                        if (await ticketService.OrderExistsInDatabase(ticket.OrderId, ticket.TicketGroupId))
                            continue;

                        for (int i = 0; i < ticket.Quantity; i++)
                        {
                            await ticketService.AddNewTicket(ticket);
                            newTicketsCount++;
                        }
                    }
                }

                // Stats update
                var ticketsCount = await ticketService.CountTickets();
                Preferences.Default.Set("tickets_count", ticketsCount);
                var pendingTicketsCount = await ticketService.CountPendingTickets();
                Preferences.Default.Set("pending_tickets", pendingTicketsCount);
                // Last sync date update
                QueryDate = DateTime.Now;
                Preferences.Default.Set("last_db_sync", QueryDate);

                if (newTicketsCount > 0)
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu.", "OK");
                else
                    await Shell.Current.DisplayAlert("Pobieranie biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy danych sklepu. Status API: {soteshopService.StatusMessage}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd przy pobieraniu biletów ze sklepu: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task SyncTicketsDatabase()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Synchronizacja biletów", $"Brak połączenia z Internetem. Połącz się z Wi-Fi lub włącz dane komórkowe", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                int newTicketsCount = 0;

                var groups = await ticketService.GetAllTicketGroups();

                if (groups.Count < 1)
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
                Preferences.Default.Set("tickets_count", ticketsCount);
                // Last sync date update
                QueryDate = DateTime.Now;
                Preferences.Default.Set("last_db_sync", QueryDate);

                if (newTicketsCount > 0)
                    await Shell.Current.DisplayAlert("Synchronizowanie bazy biletów", $"Pobrano/zaktualizowano {newTicketsCount} nowych biletów z bazy biletów.", "OK");
                else
                    await Shell.Current.DisplayAlert("Synchronizowanie bazy biletów", $"Pobrano {newTicketsCount} nowych biletów z bazy biletów. Status API: {soteshopService.StatusMessage}", "OK");
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

            try
            {
                if (!confirmReset)
                    return;

                // Clear tickets db
                var clearTicketGroupResult = await ticketService.ClearTicketGroupTable();
                var clearTicketsResult = await ticketService.ClearTicketsTable();

                // Stats update
                Preferences.Default.Set("tickets_count", 0);
                Preferences.Default.Set("pending_tickets", 0);
                Preferences.Default.Set("last_db_sync", DateTime.MinValue);
                Preferences.Default.Set("last_mailing", DateTime.MinValue.ToString());
                Preferences.Default.Set("IsReset", true);

                await Shell.Current.DisplayAlert("Resetowanie", $"Usunięto grup: {clearTicketGroupResult}\nUsunięto biletów: {clearTicketsResult}", "OK");

                await Shell.Current.GoToAsync("//LoginPage");
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
