using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts;

public interface ICacheAuctionsStates
{
    void Update(string auctionId, string state);
    void Remove(string auctionId);
    bool IsActive(string auctionId);
    void Initialize(IEnumerable<(string AuctionId, string State)> activeAuctions);
}
