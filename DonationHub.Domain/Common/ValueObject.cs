using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        return other is not null && ValueEquals(other);
    }
   

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValueEquals(other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(default(int), HashCode.Combine);
    }

    private bool ValueEquals(ValueObject other)
    {
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }


}