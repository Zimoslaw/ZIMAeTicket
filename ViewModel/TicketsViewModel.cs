using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class TicketsViewModel : BaseViewModel
    {
        public ObservableCollection<Ticket> Tickets { get; } = new();

        [ObservableProperty]
        public string searchPhrase;

        [ObservableProperty]
        public string chosenGroupName;

        [ObservableProperty]
        public bool isGroupChosen;

        TicketService ticketService;

        public TicketsViewModel(TicketService ticketService)
        {
            Title = "Lista biletów";
            ChosenGroupName = "> Wybierz grupę";
            IsGroupChosen = false;
            this.ticketService = ticketService;
        }

        [RelayCommand]
        async Task DisplayGroupChoicePopUp()
        {
            List<TicketGroup> groups = await ticketService.GetAllTicketGroups();

            string[] choices = new string[groups.Count];

            for (int i = 0; i < groups.Count; i++)
            {
                choices[i] = groups[i].Name;
            }

            string choice = await Shell.Current.DisplayActionSheet("Wybierz grupę:", "Anuluj", null, choices);

            if (string.IsNullOrEmpty(choice) || choice == "Anuluj")
            {
                IsGroupChosen = false;
                ChosenGroupName = "> Wybierz grupę";
            }    
            else
            {
                IsGroupChosen = true;
                ChosenGroupName = choice;
            }
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
        public async Task GetTicketsByPhrase()
        {
            if (IsBusy) // Nie wyświetlaj na nowo jeżeli widok jest zajety
                return;

            try
            {
                IsBusy = true;

                var ticketGroup = await ticketService.GetTicketGroupByName(ChosenGroupName);

                var tickets = await ticketService.GetTicketsByPhrase(ticketGroup.Id, SearchPhrase);

                if (Tickets.Count != 0)
                    Tickets.Clear();

                foreach (var ticket in tickets)
                    Tickets.Add(ticket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get tickets: {ex}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się pobrać biletów", "OK");
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
            string[] names = { "Jan Kowalski", "Anna Nowak", "Patryk Święty", "Johannes van Dijk", "Altair Ibn La Ahad" };
            var rand = new Random();
            int _orderId = rand.Next(1000, 99999);

            await ticketService.AddNewTicket(_orderId.ToString(), names[rand.Next(0, 5)]);
        }
    }
}
