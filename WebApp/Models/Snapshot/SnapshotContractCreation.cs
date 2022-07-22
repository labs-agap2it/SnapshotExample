using Nethereum.RPC.Eth.DTOs;

namespace WebApp.Models.Snapshot
{
    public class SnapshotContractCreation
    {
        public string ContractAddress { get; set; }
        public bool Success { get; set; }

        public void From(ContractCreationVO contractCreation)
        {
            Success = !contractCreation.FailedCreatingContract;
            ContractAddress = contractCreation.ContractAddress;
        }
    }
}
