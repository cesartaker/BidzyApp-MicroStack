using MediatR;

namespace microservicio_subastas.Requests;

public record ClaimPrizeRequest(Guid auctionId,string receptorName,
    string address,string deliveryMethod);
