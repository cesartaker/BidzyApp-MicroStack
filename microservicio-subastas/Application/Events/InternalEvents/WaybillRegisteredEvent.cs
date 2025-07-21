using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events.InternalEvents;

public class WaybillRegisteredEvent: INotification
{
    public Waybill Waybill { get; set; }

    public WaybillRegisteredEvent(Waybill waybill)
    {
        Waybill = waybill;
    }
}
