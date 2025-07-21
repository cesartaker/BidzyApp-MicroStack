using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions;

public class EndedAuctionException: Exception
{
    public Guid AuctionId { get; }

    public EndedAuctionException():base("La subasta ha finalizado y no acepta nuevas pujas"){}
}
