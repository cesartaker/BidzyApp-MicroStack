using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Exceptions;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObjects;

public class Email
{
    private static readonly Regex _regex = new(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", RegexOptions.Compiled);
    [BsonElement("value")]
    public string Value { get; }

    public Email(string value)
    {
        if (!_regex.IsMatch(value))
            throw new EmailFormatInvalidException(value);

        Value = value;
    }
    public override string ToString() => Value;

}

