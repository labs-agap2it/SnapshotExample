using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using System.Numerics;

namespace Crypto.Eth.Snapshot
{
    public class AddressUtils
    {
        protected readonly string _rpcUrl;
        protected readonly long _timeoutWindow;
        protected readonly int _maxThreads;

        public AddressUtils(IOptions<BlockchainOptions> options)
        {
            _rpcUrl = options.Value.RpcUrl ?? string.Empty;
            _maxThreads = options.Value.MaxThreads ?? 20;
            _timeoutWindow = options.Value.Timeout ?? 20000;
        }
    }
}
