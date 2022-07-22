using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Net;

namespace Crypto.Eth.Snapshot
{
    public abstract class BaseSnapshotUnit<TBlock> where TBlock : new()
    {
        protected ILogger<BaseSnapshotUnit<TBlock>> _logger;
        protected readonly string _rpcUrl;
        protected readonly long _timeoutWindow;
        protected readonly int _maxThreads;

        protected List<TBlock> _blocks = new();
        protected List<ProcessedBlock> _processedBlocks = new();
        protected readonly object _lock = new();

        protected long _startAt = 0;
        protected long _endAt = 0;

        protected BaseSnapshotUnit(IOptions<BlockchainOptions> options, ILogger<BaseSnapshotUnit<TBlock>> logger)
        {
            _rpcUrl = options.Value.RpcUrl??string.Empty;
            _maxThreads = options.Value.MaxThreads ?? 20;
            _timeoutWindow = options.Value.Timeout ?? 20000;
            _logger = logger;
        }

        public abstract void ProcessBlock(BlockWithTransactions block, TBlock snapshotBlock);
        public abstract void ProcessTransaction(TransactionVO transaction, TBlock snapshotBlock);
        public abstract void ProcessReceipt(TransactionReceiptVO receipt, TBlock snapshotBlock);
        public abstract void ProcessContractCreation(ContractCreationVO contractCreation, TBlock snapshotBlock);
        public abstract void ProcessLog(FilterLogVO filterLog, TBlock snapshotBlock);

        public TBlock[] GetBlocks()
        {
            return _blocks.AsReadOnly().ToArray();
        }

        public ProcessedBlock[] GetProcessedBlocks()
        {
            return _processedBlocks.AsReadOnly().ToArray();
        }

        protected virtual async Task ScanBlock(long blockNumber, TBlock block)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                                              SecurityProtocolType.Tls11 |
                                                              SecurityProtocolType.Tls;
            var web3 = new Web3(_rpcUrl);
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
            {
                steps.BlockStep.AddSynchronousProcessorHandler(b =>
                                                               ProcessBlock(b, block));
                steps.TransactionStep.AddSynchronousProcessorHandler(t =>
                                                               ProcessTransaction(t, block));
                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tr =>
                                                               ProcessReceipt(tr, block));
                steps.ContractCreationStep.AddSynchronousProcessorHandler(cc =>
                                                               ProcessContractCreation(cc, block));
                steps.FilterLogStep.AddSynchronousProcessorHandler(fl =>
                                                               ProcessLog(fl, block));
            });
            var cancellationToken = new CancellationToken();
            await processor.ExecuteAsync(blockNumber, cancellationToken, blockNumber);
            OnScanOver(block, blockNumber);
        }

        protected virtual void OnScanOver(TBlock block, long blockNumber)
        {
            lock (_lock)
            {
                var processedBlock = _processedBlocks.First(x => x.BlockNumber == blockNumber);
                processedBlock.IsCompleted = true;
                _blocks.Add(block);
            }
        }

        protected virtual async Task ExecuteScan(long startAt, long endAt)
        {
            _startAt = startAt;
            _endAt = endAt;
            while (true)
            {
                long nextBlock = -1;
                lock (_lock)
                {
                    _processedBlocks = _processedBlocks.OrderBy(x => x.BlockNumber).ToList();
                    var last = _processedBlocks.LastOrDefault();
                    nextBlock = last == null ? startAt : last.BlockNumber == endAt ? -1 : last.BlockNumber + 1;
                    if (nextBlock != -1)
                    {
                        _processedBlocks.Add(new ProcessedBlock() { BlockNumber = nextBlock, IsCompleted = false });
                    }
                }

                if (nextBlock == -1) break;
                while (true)
                {
                    try
                    {
                        await ScanBlock(nextBlock, new TBlock());
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error. " + ex.Message);
                        Thread.Sleep(Convert.ToInt32(_timeoutWindow));
                    }
                }
            }
        }

        protected virtual void Run(long startAt, long endAt, List<TBlock>? blocks, List<ProcessedBlock>? processedBlocks)
        {
            _blocks = new();
            _processedBlocks = new();
            if (blocks != null)
            {
                foreach (var block in blocks)
                    _blocks.Add(block);
            }
            if (processedBlocks != null)
            {
                foreach (var block in processedBlocks)
                {
                    if (block.IsCompleted)
                        _processedBlocks.Add(block);
                }
            }
            var blocksAlreadyProcessed = processedBlocks == null ? 0 : processedBlocks.Count;
            var threadsToSet = endAt - startAt - blocksAlreadyProcessed + 1;
            threadsToSet = threadsToSet > _maxThreads ? _maxThreads : threadsToSet;
            for (var threadCounter = 0; threadCounter < threadsToSet; threadCounter++)
            {
                Task.Run(() => ExecuteScan(startAt, endAt));
            }
        }
        public bool IsSnapshotRunning()
        {
            lock (_lock)
            {
                return _processedBlocks.Count < (_endAt - _startAt + 1) || _processedBlocks.Any(x => !x.IsCompleted);
            }
        }
    }
}
