using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    }
}
