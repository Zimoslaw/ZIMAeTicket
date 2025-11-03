using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIMAeTicket.Model
{
    [Table("ticket_groups")]
    public class TicketGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Lokalne ID grupy
        [Unique]
        public int ProductId { get; set; } // ID produktu w bazie danych Soteshop
        [Unique, MaxLength(32)]
        public string Name { get; set; } // Dowolna

        public TicketGroup() { }

        public TicketGroup(int productId, string name)
        {
            ProductId = productId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
