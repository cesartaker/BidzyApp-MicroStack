using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos;

public class CreatedAuctionResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public decimal ReservePrice { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MinBidStep { get; set; }
    public string ImageUrl { get; set; } 
    public CreatedAuctionResponseDto(Auction auction)
    {
        Id = auction.Id;
        Name = auction.Name;
        Description = auction.Description;
        BasePrice = auction.BasePrice;
        ReservePrice = auction.ReservePrice;
        EndDate = auction.EndDate;
        MinBidStep = auction.MinBidStep;
        ImageUrl = auction.ImageUrl;
    }
}
