using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataManagement
{
    public class Snapshot<TBlock, TState>
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public List<TBlock> Blocks { get; set; } = new();
        public TState? State { get; set; }
    }
}
