using System.IO.Pipelines;
using System.Text.RegularExpressions;
using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    [QueryProperty(nameof(Ticket), "Ticket")]
    public partial class TicketDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Ticket ticket;

#if WINDOWS
        [ObservableProperty]
        string resendEmailAddress;

        [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        private static partial Regex EmailAddressRegex();

#endif

        TicketService ticketService;
        MailingService mailingService;
        SoteshopService soteshopService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotUsed))]
        public bool isUsed;

        public bool IsNotUsed => !IsUsed;

#if WINDOWS
        public TicketDetailsViewModel(TicketService ticketService, MailingService mailingService, SoteshopService soteshopService)
        {
            Title = $"Szczegóły biletu";
            this.ticketService = ticketService;
            this.mailingService = mailingService;
            this.soteshopService = soteshopService;
        }
#endif
        public TicketDetailsViewModel(TicketService ticketService, MailingService mailingService)
        {
            Title = $"Szczegóły biletu";
            this.ticketService = ticketService;
            this.mailingService = mailingService;
        }

        public async Task ChangeUsedProperty()
        {
            IsUsed = Ticket.Used;
        }

        [RelayCommand]
        async Task UseTicket(Ticket ticket)
        {
            if (ticket is null)
                return;

            try
            {
                IsBusy = true;
                var result = await ticketService.UseTicket(ticket);

                if (result)
                {
                    ticket.Used = true;
                    ChangeUsedProperty();
                }
                else
                    await Shell.Current.DisplayAlert("Błąd SQLite", $"Nie udało się wykorzystać biletu nr {ticket.Id}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to use ticket: {ex}");
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się wykorzystać biletu nr {ticket.Id}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

#if WINDOWS
        [RelayCommand]
        async Task ResendTicket()
        {
            ResendEmailAddress = Ticket.OrderEmail;

            if (string.IsNullOrEmpty(ResendEmailAddress) || !EmailAddressRegex().IsMatch(ResendEmailAddress))
            {
                await Shell.Current.DisplayAlert("Wysyłanie biletu", "Wprowadzony adres nie jest poprawnym adresem e-mail", "OK");
                return;
            }

            bool initResult = mailingService.InitSMTPConnection();

            if (!initResult)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Błąd inicjalizacji z serwerem poczty: {mailingService.StatusMessage}", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                string dateOfEmail = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                TicketGroup ticketGroup = await ticketService.GetTicketGroupById(Ticket.TicketGroupId);

                mailingService.InitMessage(Ticket.OrderId, dateOfEmail, Ticket.DateOfOrder, ResendEmailAddress, Ticket.Buyer, ticketGroup.Name);

                bool attachResult = await mailingService.AttatchQRCodeToMessage(Ticket.Hash);
                if (!attachResult)
                {
                    throw new Exception(mailingService.StatusMessage);
                }

                bool sendResult = await mailingService.SendMail();
                if (!sendResult)
                {
                    throw new Exception(mailingService.StatusMessage);
                }

                Ticket.DateOfEmail = dateOfEmail;

                // Putting sent ticket into db table with ready to download and scan tickets
                bool putResult = await soteshopService.PutTicketIntoRemoteDatabase(Ticket);

                if (!putResult)
                {
                    throw new Exception(soteshopService.StatusMessage);
                }

                await ticketService.UpdateTicket(Ticket);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się wysłać biletu: {ex.Message}", "OK");
            }
            finally
            {
                mailingService.CloseSMTPConnection();

                IsBusy = false;

                await Shell.Current.DisplayAlert("Wysyłanie biletu", $"Bilet wysłany ponownie do {ResendEmailAddress}", "OK");
            }
        }
#endif
    }
}
