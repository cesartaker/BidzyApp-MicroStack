using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Role
{
    public Guid ID { get; set; }
  
    public string? Name { get; set; }
    
    public Role() { }
    public Role(string name)
    {
        ID = Guid.NewGuid();
        Name = name;
        
    }

    public Role(Guid Id,string name)
    {
        ID = Id;
        Name = name;
    }
}
