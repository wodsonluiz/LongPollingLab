using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Service.Models
{
    public class Order
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public string? Description { get; set; }
        public string? Quantidade { get; set; }
        public string? Status { get; set; }
        public string? SecretKey { get; set; }
    }
}
