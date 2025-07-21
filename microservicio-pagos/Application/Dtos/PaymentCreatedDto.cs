using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dtos;

public class PaymentCreatedDto
{
    public Guid PaymentId { get; set; }
    public Guid UserId { get; set; }
    public Guid? AuctionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public PaymentCreatedDto(Guid paymentId,Guid userId, Guid? auctionId, decimal amount, PaymentStatus status, DateTime createdAt)
    {
        PaymentId = paymentId;
        UserId = userId;
        AuctionId = auctionId;
        Amount = amount;
        Status = status;
        CreatedAt = createdAt;
    }
}
