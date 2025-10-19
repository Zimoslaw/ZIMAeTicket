namespace ZIMAeTicket.View;

public partial class TicketDetails : ContentPage
{
    private readonly TicketDetailsViewModel viewModel;

    public TicketDetails(TicketDetailsViewModel viewModel)
	{
		InitializeComponent();
        this.viewModel = viewModel;
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await viewModel.ChangeUsedProperty();
    }
}