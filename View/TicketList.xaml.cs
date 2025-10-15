namespace ZIMAeTicket.View;

public partial class TicketList : ContentPage
{
	public TicketList(TicketsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}