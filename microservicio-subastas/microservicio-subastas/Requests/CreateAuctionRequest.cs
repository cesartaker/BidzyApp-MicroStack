namespace microservicio_subastas.Requests;

public record CreateAuctionRequest(string name, string description,
    decimal basePrice, decimal reservePrice, DateTime endDate, decimal minBidStep, string imageUrl);
