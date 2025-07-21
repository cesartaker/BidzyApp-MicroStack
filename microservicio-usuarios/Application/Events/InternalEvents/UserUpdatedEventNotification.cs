using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Primitives;

namespace Application.Events.InternalEvents;

public class UserUpdatedEventNotification: INotification
{
    public User User { get; set; }
    public string Rolename { get; set; }
    public UserUpdatedEventNotification(User user,string roleName )
    {
       User = user;
       Rolename = roleName;
    }
}
