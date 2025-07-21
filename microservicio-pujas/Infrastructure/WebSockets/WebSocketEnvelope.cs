using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.WebSockets;

public class WebSocketEnvelope
{
    public string type { get; set; } = default!;
    public JsonElement payload { get; set; }
}
