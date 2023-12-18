using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Models
{
    public class Order
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }
    }

    public class OrderEvent
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }
    }
}
