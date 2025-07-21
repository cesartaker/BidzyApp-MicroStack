using Domain.Entities;

namespace Application.Dtos.AuctionResults;

public class AuctionsDto
{
    public List<Auction> Auctions { get; set; }
    public AuctionsDto(List<Auction> auctions)
    {
        Auctions = auctions;
    }
}
