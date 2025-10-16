

namespace ZIMAeTicket.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title = "ZIMAeTicket";

        public bool IsNotBusy => !IsBusy;
    }
}
