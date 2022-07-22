using Nethereum.RPC.Eth.DTOs;

namespace WebApp.Models.Snapshot
{
    public class SnapshotTransactionReceipt
    {
        public string? Error { get; set; }
        public bool Success { get; set; }

        public void From(TransactionReceiptVO receipt)
        {
            Error = receipt.Error;
            Success = receipt.Succeeded;
        }
    }
}
