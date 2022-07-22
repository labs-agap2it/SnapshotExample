using Nethereum.RPC.Eth.DTOs;

namespace WebApp.Models.Snapshot
{
    public class SnapshotTransaction
    {
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string Hash { get; set; }
        public SnapshotTransactionReceipt Receipt { get; set; }
        public List<SnapshotLog> Logs { get; set; } = new();
        public SnapshotContractCreation ContractCreation { get; set; }

        public void From(Transaction transaction)
        {
            Hash = transaction.TransactionHash;
            AddressFrom = transaction.From;
            AddressTo = transaction.To;
        }
    }
}
