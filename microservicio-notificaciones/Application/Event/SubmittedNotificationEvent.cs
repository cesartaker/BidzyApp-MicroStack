using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Event;
public class SubmittedNotificationEvent: INotification
{
    public Notification Notification { get; set; }

    public SubmittedNotificationEvent(Notification notification)
    {
        Notification = notification ?? throw new ArgumentNullException(nameof(notification), "Notification cannot be null");
    }
}
