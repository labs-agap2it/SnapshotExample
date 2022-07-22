using Crypto.Eth.Snapshot;
using Crypto.Eth.Snapshot.Model;
using Microsoft.Extensions.Options;
using WebApp.Models.Snapshot;

namespace WebApp.SnapshotUnits
{
    public class TokenSnapshotUnit : BaseSnapshotUnit<SnapshotBlock>
    {
        public TokenSnapshotUnit(IOptions<BlockchainOptions> options, ILogger<TokenSnapshotUnit> logger) : base(options, logger)
        {
        }
    }
}
