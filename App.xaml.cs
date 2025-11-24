namespace ZIMAeTicket
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Height = 800,
                Width = 800
            };
        }
    }
}