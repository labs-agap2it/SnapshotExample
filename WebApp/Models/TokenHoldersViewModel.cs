namespace WebApp.Models
{

    public class Holder
    {
        public string? Address { get; set; }
        public long Amount { get; set; }
    }
    public class TokenHoldersViewModel
    {
        public List<Holder> Holders { get; set; }
    }
}
