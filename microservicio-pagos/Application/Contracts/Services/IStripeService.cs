using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Stripe;

namespace Application.Contracts.Services;

public interface IStripeService
{
    Task<PaymentResultDto> SendPaymentAsync(string paymentMethodId, decimal amount, string currency);
    PaymentMethod GetPaymentMethod(string paymentMethodId);
}
