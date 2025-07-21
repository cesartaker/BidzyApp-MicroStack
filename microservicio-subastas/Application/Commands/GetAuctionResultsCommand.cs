using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;
using Domain.Enums;
using MediatR;

namespace Application.Commands;

public record GetAuctionResultsCommand(Guid userId, List<string> statuses):IRequest<List<AuctionResultDto>>;
