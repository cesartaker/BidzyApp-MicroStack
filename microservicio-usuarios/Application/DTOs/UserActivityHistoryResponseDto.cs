using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs;

public class UserActivityHistoryResponseDto
{
    public List<UserActivityHistory> History { get; set;}

    public UserActivityHistoryResponseDto(List<UserActivityHistory> history)
    {
        History = history;
    }
}
