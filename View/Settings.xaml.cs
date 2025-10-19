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
}