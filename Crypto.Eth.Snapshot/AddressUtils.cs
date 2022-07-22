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

        public bool IsValidAddress(string address)
        {
            var addressUtil = new Nethereum.Util.AddressUtil();
            return addressUtil.IsValidEthereumAddressHexFormat(address);
        }

        public async Task<bool> IsContractAddress(string address)
        {
            try
            {
                var web3 = new Web3(_rpcUrl);
                var code = await web3.Eth.GetCode.SendRequestAsync(address);
                return code != "0x";
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsTokenAddress(string address)
        {
            var web3 = new Web3(_rpcUrl);
            var abi = @"[{""inputs"": [], ""name"": ""totalSupply"", ""outputs"": [ 
                {""internalType"": ""uint256"", ""name"": """", 
                 ""type"": ""uint256"" } ],""stateMutability"": ""view"",
                 ""type"": ""function"" }]";
            var contract = web3.Eth.GetContract(abi, address);
            try
            {
                var totalSupplyFunction = contract.GetFunction("totalSupply");
                var totalSupply = await totalSupplyFunction.CallAsync<BigInteger>();
                return totalSupply != 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
