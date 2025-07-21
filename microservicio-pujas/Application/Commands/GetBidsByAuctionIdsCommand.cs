using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record GetBidsByAuctionIdsCommand(List<Guid> auctionIds):IRequest<List<Bid>>;
