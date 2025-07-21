using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Event;
public class PaymentUpdatedEvent:INotification
{
    public Payment Payment { get; set; }
    public PaymentUpdatedEvent(Payment payment)
    {
        Payment = payment;
    }
}
