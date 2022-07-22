namespace WebApp.Models
{
    public class SnapshotRequest
    {
        public string? Address { get; set; }
        public string? Name { get; set; }
        public int StartBlock { get; set; }
        public int EndBlock { get; set; }
    }
}
