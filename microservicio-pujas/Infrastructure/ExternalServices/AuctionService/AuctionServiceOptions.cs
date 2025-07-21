using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.ExternalServices.AuctionService;

public class AuctionServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
