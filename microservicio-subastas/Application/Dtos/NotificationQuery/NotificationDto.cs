using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.NotificationQuery;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string winnerSubject { get; set; }
    public string auctioneerSubject { get; set; } 
    public string auctioneerMessage { get; set; }
    public string winnerMessage { get; set; }
    public string winnerEmail { get; set; }
    public string auctioneerEmail { get; set; }
}
