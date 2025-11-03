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
                conn.CreateTableAsync<TicketGroup>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed initializing database: {ex.Message}");
            }
        }

        public async Task<List<Ticket>> GetTicketsByPhrase(int ticketGroupId, string searchPhrase)
        {
            if (searchPhrase is null || searchPhrase.Length < 3)
                return new List<Ticket>();

            try
            {
                return await conn.Table<Ticket>().Where(t =>
                                                        t.TicketGroupId == ticketGroupId &&
                                                        t.Buyer.ToLower().Contains(searchPhrase.ToLower())).ToListAsync();
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

                StatusMessage = string.Format("Ticket with ID {0} used.", ticket.Id);

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
            int ticketGroupId = 2)
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

                StatusMessage = string.Format("{0} record(s) added.", result.Result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add record(s): {0}", ex.Message);
            }
        }

        public async Task<bool> AddNewTicketGroup(int productId, string groupName)
        {
            Task<int> result;

            try
            {
                if (string.IsNullOrEmpty(groupName) || productId < 1)
                {
                    throw new Exception("Cannot add group with empty name or product ID");
                }

                result = conn.InsertAsync(new TicketGroup
                {
                    ProductId = productId,
                    Name = groupName
                });

                StatusMessage = string.Format("{0} record(s) added.", result.Result);

                if (result.Result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add record(s): {0}", ex.Message);
                return false;
            }
        }

        public async Task<List<TicketGroup>> GetAllTicketGroups()
        {
            try
            {
                return await conn.Table<TicketGroup>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return new List<TicketGroup>();
            }
        }

        public async Task<TicketGroup> GetTicketGroupByName(string name)
        {
            try
            {
                return await conn.Table<TicketGroup>().Where(g => g.Name.ToLower().Contains(name.ToLower())).FirstAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return null;
            }
        }

        // CZYSZCZENIE TABELI BILETÓW
        public async Task<int> ClearTicketsTable()
        {
            return await conn.ExecuteAsync("DELETE FROM tickets");
        }

        // CZYSZCZENIE TABELI GRUP BILETÓW
        public async Task<int> ClearTicketGroupTable()
        {
            return await conn.ExecuteAsync("DELETE FROM ticket_groups");
        }
    }
}
