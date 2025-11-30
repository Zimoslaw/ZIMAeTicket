using ZIMAeTicket.View;

namespace ZIMAeTicket
{
    public partial class AppShell : Shell
    {
        bool loginRequired;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("LoginPage", typeof(View.LoginPage));

            Routing.RegisterRoute("TicketDetails", typeof(View.TicketDetails));

            Routing.RegisterRoute("NewTicketGroup", typeof(View.NewTicketGroup));

            Routing.RegisterRoute("ScanQR", typeof(View.ScanQR));
        }
    }
}
