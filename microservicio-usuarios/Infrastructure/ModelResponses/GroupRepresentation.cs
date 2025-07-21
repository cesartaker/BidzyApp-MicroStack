using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ModelResponses;

public class GroupRepresentation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public List<GroupRepresentation> SubGroups { get; set; }
}
