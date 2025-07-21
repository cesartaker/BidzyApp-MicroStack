
using System.Net.WebSockets;
using Application.Dtos;


namespace Application.Contracts;

public interface IBidsSocketHub
{
    Task HandleWebSocketAsync(WebSocket socket, string auctionId, string bidderId,string userName);
    Task BroadcastAutoBidAsync(string auctionId, BidResponseDto bid);
    Task CloseSocketsForAuctionAsync(string auctionId);
}
