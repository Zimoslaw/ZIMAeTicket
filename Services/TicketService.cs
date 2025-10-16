using SQLite;

namespace ZIMAeTicket.Services
{
    public partial class TicketService
    {
        string _dbPath;

        public string StatusMessage { get; set; }

        private SQLiteConnection conn;

        private void InitDB()
        {
            if (conn != null)
                return;

            conn = new SQLiteConnection(_dbPath);
        }

        public TicketService()
        {
            _dbPath = Constants.DatabasePath;

            try
            {
                InitDB();
                conn.CreateTable<Ticket>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed initializing database: {ex.Message}");
            }
        }

        Ticket ticket;

        public async Task<List<Ticket>> GetTicketsByPhrase(int ticketGroupId, string searchPhrase = null)
        {
            try
            {
                return conn.Table<Ticket>().ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Ticket>();
        }

        public async Task<bool> UseTicket(Ticket ticket)
        {
            try
            {
                ticket.Used = true;

                conn.Update(ticket);

                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to use ticket {0}: {1}", ticket.Id, ex.Message);
                return false;
            }
        }

        public async Task AddNewTicket(
            
            string orderId = "12345",
            string buyer = "Jan Kowalski",
            string orderEmail = "test@example.com",
            string dateOfOrder = "2025-10-14",
            string dateOfPayment = "2025-10-14",
            int ticketGroupId = 1)
        {
            int result = 0;

            try
            {
                result = conn.Insert(new Ticket {
                    TicketGroupId = ticketGroupId,
                    OrderId = orderId,
                    OrderEmail = orderEmail,
                    Buyer = buyer,
                    DateOfOrder = dateOfOrder,
                    DateOfPayment = dateOfPayment});

                StatusMessage = string.Format("{0} record(s) added", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add record(s): {0}", ex.Message);
            }
        }
    }
}
