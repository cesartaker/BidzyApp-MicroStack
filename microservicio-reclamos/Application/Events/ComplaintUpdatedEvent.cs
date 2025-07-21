using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events;

public class ComplaintUpdatedEvent:INotification
{
    public Complaint Complaint { get; set; }

    public ComplaintUpdatedEvent(Complaint complaint)
    {
        Complaint = complaint;
    }
}
