using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands;

public record ClaimPrizeCommand(Guid userId,Guid auctionId, string receptorName,
    string address, string deliveryMethod) :IRequest<Unit>;
