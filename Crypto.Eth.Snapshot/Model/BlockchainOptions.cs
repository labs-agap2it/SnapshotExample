namespace Crypto.Eth.Snapshot.Model
{
    public class BlockchainOptions
    {
        public string? RpcUrl { get; set; }
        public long? Timeout { get; set; }
        public int? MaxThreads { get; set; }
    }
}
