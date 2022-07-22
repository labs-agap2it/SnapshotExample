using Nethereum.RPC.Eth.DTOs;

namespace WebApp.Models.Snapshot
{
    public class SnapshotBlock
    {
        public long Number { get; set; }
        public string Hash { get; set; }
        public string TimeStamp { get; set; }
        public List<SnapshotTransaction> Transactions { get; set; } = new();

        public void From(BlockWithTransactions block)
        {
            Hash = block.BlockHash;
            Number = (long)block.Number.Value;
            TimeStamp = block.Timestamp.Value.ToString();
        }
    }
}
