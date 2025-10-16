namespace ZIMAeTicket.ViewModel
{
    [QueryProperty(nameof(Ticket), "Ticket")]
    public partial class TicketDetailsViewModel : BaseViewModel
    {
        public TicketDetailsViewModel()
        {
            Title = $"Szczegóły biletu";
        }

        [ObservableProperty]
        Ticket ticket;
    }
}
