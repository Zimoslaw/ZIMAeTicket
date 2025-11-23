using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZIMAeTicket.Services
{
    public partial class SoteshopService
    {
        readonly HttpClient _httpClient;

        readonly JsonSerializerOptions _jsonDeserializeOptions;

        public string StatusMessage { get; set; }
        public SoteshopService()
        {

            _httpClient = new();

            _jsonDeserializeOptions = new()
            {
                PropertyNameCaseInsensitive = true,
            };

            StatusMessage = "Initialized";
        }

        public async Task<List<Ticket>> GetTicketsFromShopByDate(int productId, DateTime dateFrom)
        {
            try
            {
                FormUrlEncodedContent postData = new(
                [
                    new KeyValuePair<string, string>("apikey", AccessStrings.ApiKey),
                    new KeyValuePair<string, string>("datetime", dateFrom.ToString("yyyy-MM-dd HH:mm:ss")),
                    new KeyValuePair<string, string>("id", productId.ToString())
                ]);

                var response = await _httpClient.PostAsync(AccessStrings.GetTicketsFromOrdersByDateApiURL, postData);

                response.EnsureSuccessStatusCode();
                StatusMessage = "OK (http 200)";

                var ticketsData = await response.Content.ReadAsStreamAsync();

                // Response serialization
                var responseTickets = JsonSerializer.Deserialize<GetTicketsByDateResponse>(ticketsData, _jsonDeserializeOptions);

                return responseTickets == null ? throw new Exception("Deserializing returned null") : responseTickets.Tickets;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error getting data from API: {ex.Message}";
                return [];
            }
        }

        public async Task<bool> PutTicketIntoRemoteDatabase(Ticket ticket)
        {
            try
            {
                FormUrlEncodedContent postData = new(
                [
                    new KeyValuePair<string, string>("apikey", AccessStrings.ApiKey),
                    new KeyValuePair<string, string>("id", ticket.Id.ToString()),
                    new KeyValuePair<string, string>("ticketgroupid", ticket.TicketGroupId.ToString()),
                    new KeyValuePair<string, string>("orderid", ticket.OrderId),
                    new KeyValuePair<string, string>("orderemail", ticket.OrderEmail),
                    new KeyValuePair<string, string>("buyer", ticket.Buyer),
                    new KeyValuePair<string, string>("dateoforder", ticket.DateOfOrder),
                    new KeyValuePair<string, string>("dateofpayment", ticket.DateOfPayment),
                    new KeyValuePair<string, string>("dateofemail", ticket.DateOfEmail),
                    new KeyValuePair<string, string>("hash", ticket.Hash),
                ]);

                var response = await _httpClient.PostAsync(AccessStrings.PutTicketApiURL, postData);

                response.EnsureSuccessStatusCode();
                StatusMessage = "OK (http 200)";

                var result = await response.Content.ReadAsStreamAsync();

                Debug.WriteLine(result);

                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error putting data via API: {ex.Message}";
                return false;
            }
        }

        public async Task<List<Ticket>> GetTicketsByDate(DateTime dateFrom)
        {
            try
            {
                FormUrlEncodedContent postData = new(
                [
                    new KeyValuePair<string, string>("apikey", AccessStrings.ApiKey),
                    new KeyValuePair<string, string>("datetime", dateFrom.ToString("yyyy-MM-dd HH:mm:ss"))
                ]);

                var response = await _httpClient.PostAsync(AccessStrings.GetTicketsByDateApiURL, postData);

                response.EnsureSuccessStatusCode();
                StatusMessage = "OK (http 200)";

                var ticketsData = await response.Content.ReadAsStreamAsync();

                // Response serialization
                var responseTickets = JsonSerializer.Deserialize<GetTicketsByDateResponse>(ticketsData, _jsonDeserializeOptions);

                return responseTickets == null ? throw new Exception("Deserializing returned null") : responseTickets.Tickets;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error getting data from API: {ex.Message}";
                return [];
            }
        }
    }

    public class GetTicketsByDateResponse
    {
        required public List<Ticket> Tickets { get; set; }
    }
}
