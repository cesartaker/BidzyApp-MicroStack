using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;
using MediatR;

namespace Application.Commands;

public record GetMyAuctionsCommand(Guid userId): IRequest<AuctionsDto>;
