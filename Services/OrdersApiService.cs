using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

public class OrdersApiService
{
    public async Task<List<Order>> GetOrdersFromApi()
    {
        using (var httpClient = new HttpClient())
        {
            string url = "https://rafaelielodemos-dev.outsystems.app/Quantum360/rest/Orders/GetOrders";
            var response = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<Order>>(response);
        }
    }

    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? OrderNumber { get; set; }
        public int Status { get; set; }
        public bool NeedsManagerApproval { get; set; }
        public int EmployeeId { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequiredDate { get; set; }
        public double Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCountryId { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipPostalCode { get; set; }
        public double TotalAmountCache { get; set; }
        public DateTime LastModified { get; set; }
    }
}

    public class MongoOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? OrderNumber { get; set; }
        public int Status { get; set; }
        public bool NeedsManagerApproval { get; set; }
        public int EmployeeId { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public DateTime RequiredDate { get; set; }
        public double Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCountryId { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipPostalCode { get; set; }
        public double TotalAmountCache { get; set; }
        public DateTime LastModified { get; set; }
    }
