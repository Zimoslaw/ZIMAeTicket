using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class TicketsViewModel : BaseViewModel
    {
        public ObservableCollection<Ticket> Tickets { get; } = new();

        TicketService ticketService;

        public TicketsViewModel(TicketService ticketService)
        {
            Title = "Lista biletów";
            this.ticketService = ticketService;
        }

        [RelayCommand]
        async Task GoToDetails(Ticket ticket)
        {
            if (ticket is null)
                return;

            await Shell.Current.GoToAsync("TicketDetails", true,
                new Dictionary<string, object>
                {
                    {"Ticket", ticket }
                });
        }

        [RelayCommand]
        async Task GetTicketsByPhrase()
        {
            if (IsBusy) // Nie wyświetlaj na nowo jeżeli widok jest zajety
                return;

            try
            {
                IsBusy = true;
                var tickets = await ticketService.GetTicketsByPhrase(1);

                if (Tickets.Count != 0)
                    Tickets.Clear();

                foreach (var ticket in tickets)
                    Tickets.Add(ticket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get tickets: {ex}");
                await Shell.Current.DisplayAlert("Error", "Unable to get tickets", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }



        // TEST
        [RelayCommand]
        async Task AddNewTicketTest()
        {
            await ticketService.AddNewTicket();
        }
    }
}
