using ZIMAeTicket.Services;

namespace ZIMAeTicket.ViewModel
{
    public partial class NewTicketGroupViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string groupName;

        [ObservableProperty]
        public string productId;

        [ObservableProperty]
        public bool isNotAdded = true;

        [ObservableProperty]
        public string addButtonText = "Dodaj";

        readonly TicketService ticketService;

        public NewTicketGroupViewModel(TicketService ticketService)
        {
            Title = "Dodaj nową grupę biletów";
            this.ticketService = ticketService;

            GroupName = string.Empty;
            ProductId = string.Empty;
        }

        [RelayCommand]
        public async Task AddNewTicketGroup()
        {
            if (IsBusy) // Nie wyświetlaj na nowo jeżeli widok jest zajety
                return;

            if (string.IsNullOrEmpty(GroupName) || string.IsNullOrEmpty(ProductId))
            {
                await Shell.Current.DisplayAlert("Błąd w formularzu", $"Uzupełnij wszystkie pola", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var result = await ticketService.AddNewTicketGroup(int.Parse(ProductId), GroupName);

                if (!result)
                    await Shell.Current.DisplayAlert("Błąd SQLite", $"Nie udało się dodać grupy biletów: {GroupName}", "OK");
                else
                {
                    IsNotAdded = false;
                    AddButtonText = "Dodano!";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add ticket group: {ex}");
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się dodać grupy biletów", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
