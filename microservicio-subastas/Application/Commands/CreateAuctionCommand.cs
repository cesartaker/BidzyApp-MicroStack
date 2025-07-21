using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;

namespace Application.Commands;

public record CreateAuctionCommand(Guid userId, Guid productId, string name, string description,
    decimal basePrice, decimal reservePrice, DateTime endDate, decimal minBidStep,string imageUrl):IRequest<CreatedAuctionResponseDto>;
