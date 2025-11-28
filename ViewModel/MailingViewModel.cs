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

        readonly TicketService ticketService;
        readonly SoteshopService soteshopService;
        readonly MailingService mailingService;

        public MailingViewModel(TicketService ticketService, SoteshopService soteshopService, MailingService mailingService)
        {
            Title = "Wyślij bilety";
            this.ticketService = ticketService;
            this.soteshopService = soteshopService;
            this.mailingService = mailingService;

            lastMailingDateTime = DateTime.MinValue.ToString();
        }

#if WINDOWS
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

            bool initResult = mailingService.InitSMTPConnection();

            if (!initResult)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd inicjalizacji z serwerem poczty: {mailingService.StatusMessage}", "OK");
                return;
            }

            if (!mailingService.SmtpClient.IsAuthenticated)
            {
                await Shell.Current.DisplayAlert("Błąd", "Błąd połączenia lub autoryzacji z serwerem poczty", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                List<string> orders = await ticketService.GetTicketsOrders();

                // One e-mail for each order
                foreach (string order in orders)
                {
                    string dateOfEmail = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    List<Ticket> tickets = await ticketService.GetTicketsByOrderId(order);

                    byte index = 0;
                    foreach(Ticket ticket in tickets)
                    {
                        // Get order data from first ticket
                        if (index == 0)
                        {
                            TicketGroup ticketGroup = await ticketService.GetTicketGroupById(ticket.TicketGroupId);
                            mailingService.InitMessage(order, dateOfEmail, ticket.DateOfOrder, ticket.OrderEmail, ticket.Buyer, ticketGroup.Name);
                            index++;
                        }

                        // Calculate hash value for QR code
                        ticket.CalculateHash();

                        bool attachResult = await mailingService.AttatchQRCodeToMessage(ticket.Id, ticket.Hash);

                        if (!attachResult)
                        {
                            throw new Exception(mailingService.StatusMessage);
                        }
                    }

                    bool sendResult = await mailingService.SendMail();

                    if (!sendResult) 
                    {
                        throw new Exception(mailingService.StatusMessage);
                    }

                    foreach (Ticket ticket in tickets)
                    {
                        ticket.DateOfEmail = dateOfEmail;

                        // Putting sent ticket into db table with ready to download and scan tickets
                        bool putResult = await soteshopService.PutTicketIntoRemoteDatabase(ticket);

                        if (putResult)
                        {
                            await ticketService.UpdateTicket(ticket);
                            SentTicketsCount++;
                        }
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
                PendingTicketsCount -= SentTicketsCount;
                Preferences.Default.Set("pending_tickets", PendingTicketsCount);

                LastMailingDateTime = DateTime.Now.ToString();
                Preferences.Default.Set("last_mailing", LastMailingDateTime);

                mailingService.CloseSMTPConnection();

                IsBusy = false;

                SentTicketsCount = 0;

                if (PendingTicketsCount != 0)
                    await Shell.Current.DisplayAlert("Coś poszło nie tak", "Nie wszystkie bilety zostały wysłane, spróbuj ponownie", "OK");
            }
        }
#endif
    }
}
