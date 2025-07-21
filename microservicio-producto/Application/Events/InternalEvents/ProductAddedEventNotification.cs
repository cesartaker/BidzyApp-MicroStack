using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events.InternalEvents;

public class ProductAddedEventNotification: INotification
{
    public Product Product { get; }

    public ProductAddedEventNotification(Product product)
    {
        Product = product;
    }
}
