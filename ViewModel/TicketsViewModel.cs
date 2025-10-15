using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class TicketsViewModel : BaseViewModel
    {
        TicketService ticketService;

        public ObservableCollection<Ticket> Tickets { get; } = new();

        public Command GetTicketsByPhraseCommand { get; }
        public Command AddNewTicketTestCommand { get; }

        public TicketsViewModel(TicketService ticketService)
        {
            Title = "Lista biletów";
            this.ticketService = ticketService;
            GetTicketsByPhraseCommand = new Command(async () => await GetTicketsByPhrase());

            // TEST
            AddNewTicketTestCommand = new Command(async () => await AddNewTicketTest());
        }

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
        async Task AddNewTicketTest()
        {
            await ticketService.AddNewTicket();
        }
    }
}
