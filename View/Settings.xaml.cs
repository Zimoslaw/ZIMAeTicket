namespace ZIMAeTicket.View;

public partial class Settings : ContentPage
{
	private readonly SettingsViewModel viewModel;

	public Settings(SettingsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        viewModel.QueryDate = Preferences.Default.Get("last_db_sync", DateTime.Now.AddMonths(-1));
    }
}