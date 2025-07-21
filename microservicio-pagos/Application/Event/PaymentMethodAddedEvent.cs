using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Event;

public class PaymentMethodAddedEvent: INotification
{
    public UserPaymentMethod UserPaymentMethod { get; set; }

    public PaymentMethodAddedEvent(UserPaymentMethod paymentMethod)
    {
        UserPaymentMethod = paymentMethod;
    }
}
