namespace ZIMAeTicket.View;

public partial class TicketList : ContentPage
{
    private readonly TicketsViewModel viewModel;

    public TicketList(TicketsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}