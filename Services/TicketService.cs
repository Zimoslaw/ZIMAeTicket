using SQLite;

namespace ZIMAeTicket.Services
{
    public partial class TicketService
    {
        string _dbPath;

        public string StatusMessage { get; set; }

        private SQLiteAsyncConnection conn;

        private void InitDB()
        {
            if (conn != null)
                return;

            conn = new SQLiteAsyncConnection(_dbPath);
        }

        public TicketService()
        {
            _dbPath = Constants.DatabasePath;

            try
            {
                InitDB();
                conn.CreateTableAsync<Ticket>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed initializing database: {ex.Message}");
            }
        }

        Ticket ticket;

        public async Task<List<Ticket>> GetTicketsByPhrase(int ticketGroupId, string searchPhrase)
        {
            if (searchPhrase is null || searchPhrase.Length < 3)
                return new List<Ticket>();

            try
            {
                return await conn.Table<Ticket>().Where(t => t.Buyer.ToLower().Contains(searchPhrase.ToLower())).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return new List<Ticket>();
            }
        }

        public async Task<bool> UseTicket(Ticket ticket)
        {
            try
            {
                ticket.Used = true;

                await conn.UpdateAsync(ticket);

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
            Task<int> result;

            try
            {
                result = conn.InsertAsync(new Ticket {
                    TicketGroupId = ticketGroupId,
                    OrderId = orderId,
                    OrderEmail = orderEmail,
                    Buyer = buyer,
                    DateOfOrder = dateOfOrder,
                    DateOfPayment = dateOfPayment});

                StatusMessage = string.Format("{0} record(s) added", result.Result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add record(s): {0}", ex.Message);
            }
        }
    }
}
