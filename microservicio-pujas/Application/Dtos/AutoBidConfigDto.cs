using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;
public class AutoBidConfigDto
{
    public decimal MaxAmount { get; set; }
    public decimal MinBidIncrease { get; set; }
}
