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
        HttpClient _httpClient;

        JsonSerializerOptions _jsonDeserializeOptions;

        public string StatusMessage { get; set; }
        public SoteshopService()
        {

            _httpClient = new();

            _jsonDeserializeOptions = new()
            {
                PropertyNameCaseInsensitive = true,
            };
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

                var response = await _httpClient.PostAsync(AccessStrings.GetTicketsByDateApiURL, postData);

                response.EnsureSuccessStatusCode();
                StatusMessage = "OK (http 200)";

                var ticketsData = await response.Content.ReadAsStreamAsync();

                // Response serialization
                var responseTickets = JsonSerializer.Deserialize<GetTicketsFromShopByDateResponse>(ticketsData, _jsonDeserializeOptions);

                return responseTickets.Tickets;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error getting data from API: {ex.Message}";
                return new List<Ticket>();
            }
        }
    }

    public class GetTicketsFromShopByDateResponse
    {
        public List<Ticket> Tickets { get; set; }
    }
}
