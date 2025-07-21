using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events.InternalEvents;

public class ProductUpdatedEventNotification: INotification
{
    public Product Product { get; }

    public ProductUpdatedEventNotification(Product product)
    {
        Product = product;
    }
}
