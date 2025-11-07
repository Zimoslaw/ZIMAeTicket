using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class MailingViewModel : BaseViewModel
    {
        [ObservableProperty]
        public int pendingTicketsCount;

        [ObservableProperty]
        public int sentTicketsCount = 0;

        [ObservableProperty]
        public double sendingProgress = 0;

        [ObservableProperty]
        public DateTime lastMailingDateTime;

        TicketService ticketService;

        public MailingViewModel(TicketService ticketService)
        {
            Title = "Wyślij bilety";
            this.ticketService = ticketService;
        }

        [RelayCommand]
        async Task SendTicketsByEmail()
        {
            IsBusy = true;

            List<Ticket> ticketsToSend = await ticketService.GetTicketsToSend();

            // Calculate hash values for QR codes
            foreach (Ticket ticket in ticketsToSend)
            {
                ticket.CalculateHash();
            }

            try
            {
                foreach (Ticket ticket in ticketsToSend)
                {
                    SentTicketsCount++;
                    SendingProgress = SentTicketsCount / PendingTicketsCount;
                    // TODO generating QR code
                    // TODO sending emails
                }
                
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd przy wysyłaniu biletów: {ex.Message}", "OK");
            }
            finally
            {
                PendingTicketsCount = PendingTicketsCount - SentTicketsCount;
                Preferences.Set("pending_tickets", PendingTicketsCount);
                SentTicketsCount = 0;
                LastMailingDateTime = DateTime.Now;
                Preferences.Set("last_mailing", LastMailingDateTime);
                IsBusy = false;
            }
        }
    }
}
