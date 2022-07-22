using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;

namespace WebApp.Models.Snapshot
{
    public class SnapshotLog
    {
        public string Data { get; set; }
        public string Address { get; set; }
        public int BlockLogIndex { get; set; }
        public string Name { get; set; }    

        public void From(FilterLog log, string name)
        {
            Data = log.Data;
            Address = log.Address;
            BlockLogIndex = (int)log.LogIndex.Value;
            Name = name;   
        }
    }
}
