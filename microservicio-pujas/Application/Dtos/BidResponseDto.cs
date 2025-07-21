using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public class BidResponseDto
{
    public string bidderName { get; set; }
    public decimal amount { get; set; }
}
