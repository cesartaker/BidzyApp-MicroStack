using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public class AuctionUserDto
{
    public Guid userId { get; set; }
    public string name { get; set; }
    public string email { get; set; }
}
