using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Infrastructure.ModelResponses;

public class CredentialResponse
{
    public string? id {  get; set; }
    
    public string? type { get; set; }
}
