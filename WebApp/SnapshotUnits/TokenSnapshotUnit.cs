using Crypto.Eth.Snapshot;
using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using WebApp.Models.Snapshot;

namespace WebApp.SnapshotUnits
{
    public class TokenSnapshotUnit : BaseSnapshotUnit<SnapshotBlock>
    {
        public TokenSnapshotUnit(IOptions<BlockchainOptions> options, ILogger<TokenSnapshotUnit> logger) : base(options, logger)
        {
        }

        public override void ProcessBlock(BlockWithTransactions block, SnapshotBlock snapshotBlock)
        {
            throw new NotImplementedException();
        }

        public override void ProcessContractCreation(ContractCreationVO contractCreation, SnapshotBlock snapshotBlock)
        {
            throw new NotImplementedException();
        }

        public override void ProcessLog(FilterLogVO filterLog, SnapshotBlock snapshotBlock)
        {
            throw new NotImplementedException();
        }

        public override void ProcessReceipt(TransactionReceiptVO receipt, SnapshotBlock snapshotBlock)
        {
            throw new NotImplementedException();
        }

        public override void ProcessTransaction(TransactionVO transaction, SnapshotBlock snapshotBlock)
        {
            throw new NotImplementedException();
        }
    }
}
