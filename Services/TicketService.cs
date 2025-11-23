using SQLite;

namespace ZIMAeTicket.Services
{
    public partial class TicketService
    {
        readonly string _dbPath;

        public string StatusMessage { get; set; }

        readonly private SQLiteAsyncConnection conn;

        public TicketService()
        {
            _dbPath = Constants.DatabasePath;

            conn = new SQLiteAsyncConnection(_dbPath);

            try
            {
                conn.CreateTableAsync<Ticket>();
                conn.CreateTableAsync<TicketGroup>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed initializing database: {ex.Message}");
                StatusMessage = ex.Message;
            }

            StatusMessage = "Initialized";
        }

        public async Task<List<Ticket>> GetTicketsByPhrase(int ticketGroupId, string searchPhrase)
        {
            if (searchPhrase is null || searchPhrase.Length < 3)
                return [];

            try
            {
                return await conn.Table<Ticket>().Where(t =>
                                                        t.TicketGroupId == ticketGroupId &&
                                                        t.Buyer.ToLower().Contains(searchPhrase.ToLower())).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return [];
            }
        }

        public async Task<List<Ticket>> GetTicketsToSend()
        {
            try
            {
                return await conn.Table<Ticket>().Where(t => t.DateOfEmail == string.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return [];
            }
        }

        public async Task<List<string>> GetTicketsOrders()
        {
            try
            {
                var tickets = await conn.Table<Ticket>().Where(t => t.DateOfEmail == string.Empty).ToListAsync();
                return tickets.Select(t => t.OrderId).Distinct().ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return [];
            }
        }

        public async Task<List<Ticket>> GetTicketsByOrderId(string orderId)
        {
            try
            {
                return await conn.Table<Ticket>().Where(t => t.OrderId == orderId).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return [];
            }
        }

        public async Task<Ticket> GetTicketByHash(string hash)
        {
            try
            {
                return await conn.Table<Ticket>().Where(t => t.Hash == hash).FirstAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return new Ticket();
            }
        }

        public async Task<bool> OrderExistsInDatabase(string orderId, int ticketGroupId)
        {
            try
            {
                int count = await conn.Table<Ticket>().Where(t => t.OrderId == orderId && t.TicketGroupId == ticketGroupId).CountAsync();

                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task UpdateTicket(Ticket ticket)
        {
            try
            {
                await conn.UpdateAsync(ticket);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to update data. {0}", ex.Message);
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

        public async Task AddNewTicket(Ticket ticket)
        {
            int result;

            try
            {
                result = await conn.InsertAsync(ticket);

                StatusMessage = string.Format("{0} record(s) added.", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add record(s): {0}", ex.Message);
            }
        }

        public async Task<bool> AddNewTicketGroup(int productId, string groupName)
        {
            int result;

            try
            {
                if (string.IsNullOrEmpty(groupName) || productId < 1)
                {
                    throw new Exception("Cannot add group with empty name or product ID");
                }

                result = await conn.InsertAsync(new TicketGroup
                {
                    Id = productId,
                    Name = groupName
                });

                StatusMessage = string.Format("{0} record(s) added.", result);

                if (result == 1)
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
                return [];
            }
        }

        public async Task<TicketGroup> GetTicketGroupById(int id)
        {
            try
            {
                return await conn.Table<TicketGroup>().Where(g => g.Id == id).FirstAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
                return new TicketGroup();
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
                return new TicketGroup();
            }
        }

        public async Task<int> CountTickets()
        {
            return await conn.Table<Ticket>().CountAsync();
        }

        public async Task<int> CountPendingTickets()
        {
            return await conn.Table<Ticket>().Where(t => t.DateOfEmail == string.Empty).CountAsync();
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
