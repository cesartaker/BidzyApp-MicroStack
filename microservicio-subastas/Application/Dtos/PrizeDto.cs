using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public class PrizeDto
{
    public Guid auctionId { get; set; }
    public Guid WinnerId { get; set; }
    public string auctionName { get; set; }
    public string productName { get; set; }
    public string productDescription { get; set; }
    public string productImage { get; set; }
    public bool isClaimed { get; set; }
    public bool isDelivered { get; set; }
}
