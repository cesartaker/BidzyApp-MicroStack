using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IMongoPaymentMethodReadRepository
{
    Task AddPaymentMethod(UserPaymentMethod paymentMethod);
    Task<List<UserPaymentMethod>> GetPaymentMethodsByUserId(Guid userId);
}
