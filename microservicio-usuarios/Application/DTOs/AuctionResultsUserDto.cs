using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class AuctionResultsUserDto
{
    public Guid userId { get; set; }
    public string name { get; set; }
    public string email { get; set; }
}
