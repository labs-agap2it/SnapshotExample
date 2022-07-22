using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataManagement
{
    public class SnapshotDataAccessObject<TBlock, TState>
    {
        private readonly IMongoCollection<Snapshot<TBlock, TState>> _snapshotCollection;
        
        public SnapshotDataAccessObject(IOptions<DatabaseOptions> databaseOptions)
        {
            var conn = new MongoClient(databaseOptions.Value.ConnectionString);
            var db = conn.GetDatabase(databaseOptions.Value.DatabaseName);
            _snapshotCollection = db.GetCollection<Snapshot<TBlock, TState>>("Snapshots");
        }

        /// <summary>
        /// Saves a snapshot
        /// </summary>
        public async Task<string?> CreateSnapshot(Snapshot<TBlock, TState> snapshot)
        {
            await _snapshotCollection.InsertOneAsync(snapshot);
            return snapshot?.Id;
        }

        /// <summary>
        /// Fetches a snapshot from the database
        /// </summary>
        public async Task<Snapshot<TBlock, TState>> GetSnapshot(string name)
        {
            return (await _snapshotCollection.FindAsync(x => x.Name == name)).FirstOrDefault();
        }

        /// <summary>
        /// Updates a snapshot from the database
        /// </summary>
        public async Task UpdateSnapshot(Snapshot<TBlock, TState> snapshot)
        {
            var res = await _snapshotCollection.ReplaceOneAsync((x)=>x.Name == snapshot.Name, snapshot);
        }
    }
}
