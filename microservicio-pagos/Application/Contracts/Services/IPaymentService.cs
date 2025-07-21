
using Application.Dtos;
using Domain.Entities;

namespace Application.Contracts.Services;
public interface IPaymentService
{
    Task<Payment> CreatePayment(Guid userId,Guid auctionId,decimal amount);
    Task<Payment> SendPayment(Guid paymentId, string paymentMethodId);
    Task<List<PaymentCreatedDto>> GetPendingPaymentsByUserId(Guid userId);
}
