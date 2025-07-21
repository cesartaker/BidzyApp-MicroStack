using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record GetBidsCommand(Guid auctionId):IRequest<List<Bid>>;
