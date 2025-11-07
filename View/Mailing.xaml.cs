namespace ZIMAeTicket.View;

public partial class Mailing : ContentPage
{
    private readonly MailingViewModel viewModel;

    public Mailing(MailingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        viewModel.PendingTicketsCount = Preferences.Default.Get("pending_tickets", 0);
        viewModel.LastMailingDateTime = Preferences.Default.Get("last_mailing", DateTime.MinValue);
        if (viewModel.LastMailingDateTime != DateTime.MinValue)
            LastMailingLabel.Text = viewModel.LastMailingDateTime.ToString();
        else
            LastMailingLabel.Text = "Nigdy";
    }
}