using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ZIMAeTicket.Model
{
    [Table("tickets")]
    public class Ticket
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Własne ID w bazie biletów
        public int TicketGroupId { get; set; } // ID grupy biletów (ID wydarzenia)
        [MaxLength(10)]
        public string OrderId { get; set; } // Numer zamówienia Soteshop
        public string OrderEmail { get; set; } // E-mail klienta z zamówienia
        public string Buyer {  get; set; } // Imie i Nazwisko klienta
        [MaxLength(10)]
        public string DateOfOrder { get; set; } // Data złożenia zamówienia
        [MaxLength(10)]
        public string DateOfPayment { get; set; } // Data opłacenia zamówienia
        [MaxLength(10)]
        public string DateOfEmail { get; set; } = string.Empty; // Data wysłania e-maila z biletami
        [MaxLength(10)]
        public string DateOfUsing { get; set; } = string.Empty; // Data wykorzystania biletu
        public bool Used { get; set; } // Czy bilet został wykorzystany
        [MaxLength(64)]
        public string Hash {  get; set; } = string.Empty; // Skrót biletu dla kodu QR

        public Ticket()
        {
        }

        public Ticket(int id, int ticketGroupId, string orderId, string orderEmail, string buyer, string dateOfOrder, string dateOfPayment)
        {
            Id = id;
            TicketGroupId = ticketGroupId;
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OrderEmail = orderEmail ?? throw new ArgumentNullException(nameof(orderEmail));
            Buyer = buyer ?? throw new ArgumentNullException(nameof(buyer));
            DateOfOrder = dateOfOrder ?? throw new ArgumentNullException(nameof(dateOfOrder));
            DateOfPayment = dateOfPayment ?? throw new ArgumentNullException(nameof(dateOfPayment));

            // Creating Hash of a ticket
            string ticketJSON = $"""
                                "Ticket": 
                                "[Id": {Id}, 
                                "TicketGroupId": {TicketGroupId}, 
                                "OrderId": "{OrderId}", 
                                "OrderEmail": "{OrderEmail}", 
                                "Buyer": "{Buyer}", 
                                "DateOfOrder": "{DateOfOrder}", 
                                "DateOfPayment": "{DateOfPayment}"]
                                """;
            Hash = CryptoUtils.Hash(ticketJSON);
        }
    }
}
