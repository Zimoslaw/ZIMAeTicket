using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class MailingViewModel : BaseViewModel
    {
        [ObservableProperty]
        public int pendingTicketsCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SendingProgress))]
        public int sentTicketsCount = 0;

        public double SendingProgress => SentTicketsCount / (PendingTicketsCount + 0.01);

        [ObservableProperty]
        public string lastMailingDateTime;

        TicketService ticketService;
        SoteshopService soteshopService;

        public MailingViewModel(TicketService ticketService, SoteshopService soteshopService)
        {
            Title = "Wyślij bilety";
            this.ticketService = ticketService;
            this.soteshopService = soteshopService;
        }

        [RelayCommand]
        async Task SendTicketsByEmail()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Wysyłanie biletów", $"Brak połączenia z Internetem. Połącz się z Wi-Fi lub włącz dane komórkowe", "OK");
                return;
            }

            if (PendingTicketsCount < 1)
            {
                await Shell.Current.DisplayAlert("Wysyłanie biletów", "Brak biletów do wysłania", "OK");
                return;
            }

            IsBusy = true;

            List<Ticket> ticketsToSend = await ticketService.GetTicketsToSend();

            try
            {
                foreach (Ticket ticket in ticketsToSend)
                {
                    // Calculate hash value for QR code
                    ticket.CalculateHash();

                    // TODO generate QR code
                    // TODO send email

                    ticket.DateOfEmail = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Putting sent ticket into table with ready to download and scan tickets
                    bool putResult = await soteshopService.PutTicketIntoRemoteDatabase(ticket);

                    if (putResult)
                    {
                        await ticketService.UpdateTicket(ticket);
                        SentTicketsCount++;
                    }
                }

                await Shell.Current.DisplayAlert("Wysyłanie biletów", $"Wysłano {SentTicketsCount} biletów", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd przy wysyłaniu biletów: {ex.Message}", "OK");
            }
            finally
            {
                PendingTicketsCount = PendingTicketsCount - SentTicketsCount;
                Preferences.Default.Set("pending_tickets", PendingTicketsCount);

                LastMailingDateTime = DateTime.Now.ToString();
                Preferences.Default.Set("last_mailing", LastMailingDateTime);

                IsBusy = false;

                SentTicketsCount = 0;

                if (PendingTicketsCount != 0)
                    await Shell.Current.DisplayAlert("Coś poszło nie tak", "Nie wszystkie bilety zostały wysłane, spróbuj ponownie", "OK");
            }
        }
    }
}
