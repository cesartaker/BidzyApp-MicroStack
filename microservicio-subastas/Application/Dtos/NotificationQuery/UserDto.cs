using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.NotificationQuery;

public class UserDto
{
    public Guid id { get; set; }
    public string fullname { get; set; }
    public string email { get; set; }
}
