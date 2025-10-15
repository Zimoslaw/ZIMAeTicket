using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIMAeTicket.Model
{
    [Table("ticket_groups")]
    internal class TicketGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Lokalne ID grupy
        [Unique]
        public int ProductId { get; set; } // ID produktu w bazie danych Soteshop
        [MaxLength(32)]
        public string Name { get; set; } = string.Empty; // Dowolna
    }
}
