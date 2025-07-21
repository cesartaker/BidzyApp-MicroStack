using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Stripe;

namespace Application.Contracts.Services;

public interface IPaymentMethodService
{
    Task<UserPaymentMethod> AddPaymentMethod(Guid userId);
    Task<List<UserPaymentMethod>> GetPaymentMethodsByUserId(Guid userId);
    PaymentMethod GetPaymentMethod();
}
