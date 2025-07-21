using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class UserActivityHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }

    public UserActivityHistory() { }
    public UserActivityHistory(Guid userId, string? description)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Description = description;
        Date = DateTime.UtcNow;
    }
}
