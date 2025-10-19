namespace ZIMAeTicket.View;

public partial class NewTicketGroup : ContentPage
{
	private readonly NewTicketGroupViewModel viewModel;

	public NewTicketGroup(NewTicketGroupViewModel viewModel)
    {
		InitializeComponent();
		this.viewModel = viewModel;
        BindingContext = viewModel;
    }
}