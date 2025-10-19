using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class SettingsViewModel : BaseViewModel
    {
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
    }
}
