namespace ZIMAeTicket
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("TicketDetails", typeof(View.TicketDetails));
        }
    }
}
