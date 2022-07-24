using Crypto.Eth.Snapshot;
using Crypto.Eth.Snapshot.DataTransferObjects;
using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using WebApp.Models.Snapshot;

namespace WebApp.SnapshotUnits
{
    public class TokenSnapshotUnit : BaseSnapshotUnit<SnapshotBlock>
    {
        public string _address = "";

        public TokenSnapshotUnit(IOptions<BlockchainOptions> options, ILogger<TokenSnapshotUnit> logger) : base(options, logger)
        {
        }

        public void Run(long startAt, long endAt, string address, List<SnapshotBlock>? blocks, List<ProcessedBlock>? processedBlocks)
        {
            _address = address;
            Run(startAt, endAt, blocks, processedBlocks);
        }

        public override void ProcessTransaction(TransactionVO transaction, SnapshotBlock snapshotBlock)
        {
        }

        public override void ProcessLog(FilterLogVO filterLog, SnapshotBlock snapshotBlock)
        {
        }

        public override void ProcessBlock(BlockWithTransactions block, SnapshotBlock snapshotBlock)
        {
            _logger.LogInformation($"Processing block #{block.Number}");

            snapshotBlock.From(block);
        }

        public override void ProcessContractCreation(ContractCreationVO contractCreation, SnapshotBlock snapshotBlock)
        {
            _logger.LogInformation($"Processing contract creation for {contractCreation.ContractAddress}");

            if (contractCreation.ContractAddress != _address) return;
            var transaction = snapshotBlock.Transactions.FirstOrDefault(x => contractCreation.TransactionHash == x.Hash);
            if (transaction == null)
            {
                transaction = new SnapshotTransaction();
                transaction.From(contractCreation.Transaction);
            }
            var cc = new SnapshotContractCreation() { ContractAddress = _address, Success = contractCreation.Succeeded };
            transaction.ContractCreation = cc;
            snapshotBlock.Transactions.Add(transaction);
        }

        public override void ProcessReceipt(TransactionReceiptVO receipt, SnapshotBlock snapshotBlock)
        {
            _logger.LogInformation($"Processing tx {receipt.TransactionHash}");

            var addresses = receipt.GetAllRelatedAddresses();
            if (addresses == null || addresses.All(x => x.ToLower() != _address.ToLower())) return;
            var transaction = snapshotBlock.Transactions.FirstOrDefault(t => t.Hash == receipt.Transaction.TransactionHash);
            if (transaction == null)
            {
                transaction = new SnapshotTransaction();
                transaction.From(receipt.Transaction);
                snapshotBlock.Transactions.Add(transaction);
            }
            var snapshotReceipt = new SnapshotTransactionReceipt();
            snapshotReceipt.From(receipt);
            transaction.Receipt = snapshotReceipt;
            var logs = receipt.TransactionReceipt.Logs.DecodeAllEvents<TransferEventDataTransferObject>();
            foreach (var log in logs)
            {
                var snapshotLog = new SnapshotLog();
                snapshotLog.From(log.Log, "Transfer");
                snapshotLog.Data = JsonSerializer.Serialize(log);
                transaction.Logs.Add(snapshotLog);
            }
        }


    }
}
