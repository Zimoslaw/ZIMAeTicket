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
        [PrimaryKey]
        public int Id { get; set; } // ID produktu w bazie danych Soteshop i jednocześnie ID grupy biletów
        [Unique, MaxLength(32)]
        public string Name { get; set; } // Dowolna

        public TicketGroup() 
        { 
            Name = string.Empty;
        }

        public TicketGroup(int productId, string name)
        {
            Id = productId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
