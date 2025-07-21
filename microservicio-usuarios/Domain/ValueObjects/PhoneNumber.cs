using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObjects;

public class PhoneNumber
{
    [BsonElement("value")]
    public string Value { get;  }

    public PhoneNumber(string value)
    {
        Console.WriteLine($" Es es el número de teléfono {value}");
        Value = value;
    }

    public override string ToString() => Value;

}
