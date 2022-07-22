namespace WebApp.Models
{
    public class Transfer
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public long Amount { get; set; }
    }

    public class TransferNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TokenTradesViewModel
    {
        public List<TransferNode> TransferNodes { get; set; }
        public List<Transfer> Transfers { get; set; }
    }
}
