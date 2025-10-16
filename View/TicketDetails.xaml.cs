namespace ZIMAeTicket.View;

public partial class TicketDetails : ContentPage
{
    public TicketDetails(TicketDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}