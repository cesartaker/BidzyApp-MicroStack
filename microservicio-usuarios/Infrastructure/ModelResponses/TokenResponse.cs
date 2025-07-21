using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ModelResponses;

public class TokenResponse
{
    public string? access_token { get; set; }
    public double expires_in { get; set; }
}
