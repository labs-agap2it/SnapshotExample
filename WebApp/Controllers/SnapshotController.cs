using Crypto.Eth.Snapshot;
using Crypto.Eth.Snapshot.Model;
using DataManagement;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Models.Snapshot;
using WebApp.SnapshotUnits;

namespace WebApp.Controllers
{
    public class SnapshotController : Controller
    {
        private readonly AddressUtils _addressUtils;
        private readonly TokenSnapshotUnit _tokenSnapshotUnit;
        private readonly SnapshotDataAccessObject<SnapshotBlock, List<ProcessedBlock>> _snapshotDataAccessObject;
        private readonly ILogger<SnapshotController> _logger;

        public SnapshotController(ILogger<SnapshotController> logger, AddressUtils addressUtils, TokenSnapshotUnit tokenSnapshotUnit, SnapshotDataAccessObject<SnapshotBlock, List<ProcessedBlock>> snapshotDataAccessObject)
        {
            _snapshotDataAccessObject = snapshotDataAccessObject;
            _addressUtils = addressUtils;
            _tokenSnapshotUnit = tokenSnapshotUnit;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TakeSnapshot([FromForm]SnapshotRequest request)
        {
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> LoadSnapshot([FromForm] SnapshotRequest request)
        {
            return View("TakeSnapshot", request);
        }

        public int BlocksProcessed()
        {
            return 3;
        }

        public async Task<AddressInfo> GetAddressInfo(string address)
        {
           
            return new AddressInfo() { IsAddress = false, IsContract = false, IsToken = false };
        }

        public bool IsSnapshotRunning()
        {
            return false;
        }

        public TokenTradesViewModel GetTokenTransfers()
        {
            return new TokenTradesViewModel()
            {
                TransferNodes = new()
                {
                    new() { Id = 1, Name = "A" },
                    new() { Id = 2, Name = "B" },
                    new() { Id = 3, Name = "C" },
                    new() { Id = 4, Name = "D" },
                    new() { Id = 5, Name = "E" }
                },
                Transfers = new()
                {
                    new Transfer() { Source = 1, Target = 4, Amount = 500 },
                    new Transfer() { Source = 2, Target = 4, Amount = 100 },
                    new Transfer() { Source = 3, Target = 1, Amount = 2500 },
                    new Transfer() { Source = 4, Target = 3, Amount = 1500 },
                    new Transfer() { Source = 4, Target = 5, Amount = 5500 }
                }
            };
        }

        public TokenHoldersViewModel GetTokenHolders()
        {
            return new TokenHoldersViewModel()
            {
                Holders= new()
                {
                    new Holder() { Address = "A", Amount = 500 },
                    new Holder() { Address = "B", Amount = 100 },
                    new Holder() { Address = "C", Amount = 2500 },
                    new Holder() { Address = "D", Amount = 1500 },
                }
            };
        }

        public ProcessedBlocksViewModel GetProcessedBlocks()
        {
            var result = new ProcessedBlocksViewModel();
            result.Blocks.Add(new ProcessedBlockViewModel() { Block = 1, Transactions = 2 });
            result.Blocks.Add(new ProcessedBlockViewModel() { Block = 2, Transactions = 0 });
            result.Blocks.Add(new ProcessedBlockViewModel() { Block = 3, Transactions = 1 });
            result.Blocks.Add(new ProcessedBlockViewModel() { Block = 4, Transactions = 0 });
            result.Blocks.Add(new ProcessedBlockViewModel() { Block = 5, Transactions = 5 });
            return result;
        }
        
    }
}
