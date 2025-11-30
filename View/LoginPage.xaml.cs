namespace ZIMAeTicket.View;

public partial class LoginPage : ContentPage
{
    bool isReset = true;

    public LoginPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        isReset = Preferences.Default.Get("IsReset", true);

        if (isReset)
        {
            ConfirmPinLabel.IsVisible = true;
            ConfirmPinEntry.IsVisible = true;
            SetNewPinButton.IsVisible = true;
            LoginButton.IsVisible = false;
        }
    }

    private async void SetNewPin(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(PinEntry.Text) || string.IsNullOrEmpty(ConfirmPinEntry.Text))
        {
            await Shell.Current.DisplayAlert("Ustawianie PINu", "Podaj nowy PIN", "OK");
            return;
        }

        if (PinEntry.Text.Length < 4)
        {
            await Shell.Current.DisplayAlert("Ustawianie PINu", "PIN musi być dłuższy niż 3 cyfry", "OK");
            return;
        }

        if (PinEntry.Text != ConfirmPinEntry.Text)
        {
            await Shell.Current.DisplayAlert("Ustawianie PINu", "PINy muszą być identyczne", "OK");
            return;
        }

        try
        {
            string newSalt = CryptoUtils.RandomLowerCaseString(32);

            string newPinHash = CryptoUtils.Hash(PinEntry.Text + newSalt);

            Preferences.Default.Set("PinSalt", newSalt);
            Preferences.Default.Set("PinHash", newPinHash);
            Preferences.Default.Set("IsReset", false);

            await Shell.Current.DisplayAlert("Ustawianie PINu", "Ustawiono nowy PIN", "OK");

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Błąd podczas ustawiania PINu: {ex.Message}", "OK");
            return;
        }
    }

    private async void Login(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(PinEntry.Text))
            {
                await Shell.Current.DisplayAlert("Logowanie", "Podaj PIN", "OK");
                return;
            }

            string salt = Preferences.Default.Get("PinSalt", "");
            string pinHash = Preferences.Default.Get("PinHash", "");

            if (string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(pinHash))
            {
                throw new Exception("PIN nie ustawiony");
            }

            string enteredPINHash = CryptoUtils.Hash(PinEntry.Text + salt);

            if (enteredPINHash == pinHash)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Logowanie", "Nieprawidłowy PIN", "OK");
                return;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Błąd podczas logowania: {ex.Message}", "OK");
            return;
        }
    }
}