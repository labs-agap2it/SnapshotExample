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
            UpdateState(request.Name, request.Address);
            if (string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.Name) ||
                request.StartBlock < 0 || request.EndBlock < 0 || request.StartBlock > request.EndBlock)
                return View("Index");
            _tokenSnapshotUnit.Run(request.StartBlock, request.EndBlock, request.Address, null, null);
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> LoadSnapshot([FromForm] SnapshotRequest request)
        {
            var snapshot = await _snapshotDataAccessObject.GetSnapshot(request.Name);
            if (snapshot == null) return View("Index");
            UpdateState(request.Name, snapshot.Address);
            _tokenSnapshotUnit.Run(request.StartBlock, request.EndBlock, snapshot.Address, snapshot.Blocks, snapshot.State);
            return View("TakeSnapshot", request);
        }

        public int BlocksProcessed()
        {
            return _tokenSnapshotUnit.GetProcessedBlocks().Count(x => x.IsCompleted);
        }

        public async Task<AddressInfo> GetAddressInfo(string address)
        {
            bool isContract = false;
            bool isToken = false;
            var isValidAddress = _addressUtils.IsValidAddress(address);
            if (isValidAddress)
            {
                isContract = await _addressUtils.IsContractAddress(address);
                if (isContract)
                {
                    isToken = await _addressUtils.IsTokenAddress(address);
                }
            }
            return new AddressInfo()
            {
                IsAddress = isValidAddress,
                IsContract = isContract,
                IsToken = isToken
            };
        }

        public bool IsSnapshotRunning()
        {
            return _tokenSnapshotUnit.IsSnapshotRunning();
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
            var blocks = _tokenSnapshotUnit.GetBlocks().OrderBy(x => x.Number).ToArray();

            foreach(var block in blocks)
            {
                result.Blocks.Add(new ProcessedBlockViewModel() { Block = block.Number, Transactions = block.Transactions.Count});
            }
            return result;
        }

        public void UpdateState(string name, string address)
        {
            Task.Run(async () =>
            {
                var snapshot = await _snapshotDataAccessObject.GetSnapshot(name);
                if (snapshot == null)
                {
                    var id = await _snapshotDataAccessObject.CreateSnapshot(new Snapshot<SnapshotBlock, List<ProcessedBlock>>() { Name = name, Address = address });
                    snapshot = new Snapshot<SnapshotBlock, List<ProcessedBlock>>() { Id = id, Name = name, Address = address };
                }

                do
                {
                    try
                    {
                        _logger.LogInformation("Starting persistence update");
                        var processedBlocks = _tokenSnapshotUnit.GetProcessedBlocks().ToList();
                        var blocks = _tokenSnapshotUnit.GetBlocks().ToList();
                        snapshot.State = processedBlocks;
                        snapshot.Blocks = blocks;
                        await _snapshotDataAccessObject.UpdateSnapshot(snapshot);
                        _logger.LogInformation("State persisted");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error. " + ex.Message);
                    }

                    Thread.Sleep(20000);
                } while (IsSnapshotRunning());
            });


        }
    }
}
