namespace WebApp.Models
{
    public class ProcessedBlocksViewModel
    {
        public string Id { get; set; }
        public List<ProcessedBlockViewModel> Blocks { get; set; } = new();
    }

    public class ProcessedBlockViewModel
    {
        public long Block { get; set; }
        public int Transactions { get; set; }
    }
}
