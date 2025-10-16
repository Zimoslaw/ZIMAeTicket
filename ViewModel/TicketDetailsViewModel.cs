using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    [QueryProperty(nameof(Ticket), "Ticket")]
    public partial class TicketDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Ticket ticket;

        TicketService ticketService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotUsed))]
        public bool isUsed;

        public bool IsNotUsed => !IsUsed;

        public TicketDetailsViewModel(TicketService ticketService)
        {
            Title = $"Szczegóły biletu";
            this.ticketService = ticketService;
        }

        public async Task ChangeUsedProperty()
        {
            IsUsed = ticket.Used;
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
    }
}
