using System;
using JunkyardClicker.Core;

public readonly struct Currency
{
    public readonly double Value;

    public Currency(double value)
    {
        if (value < 0)
        {
            throw new Exception("Currency 값은 0보다 작을 수 없습니다.");
        }

        Value = value;
    }

    public static Currency operator +(Currency a, Currency b)
    {
        return new Currency(a.Value + b.Value);
    }

    public static Currency operator -(Currency a, Currency b)
    {
        return new Currency(a.Value - b.Value);
    }

    public static bool operator >=(Currency a, Currency b)
    {
        return a.Value >= b.Value;
    }

    public static bool operator <=(Currency a, Currency b)
    {
        return a.Value <= b.Value;
    }

    public static bool operator >(Currency a, Currency b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(Currency a, Currency b)
    {
        return a.Value < b.Value;
    }

    public static implicit operator Currency(double value)
    {
        return new Currency(value);
    }

    public static explicit operator double(Currency currency)
    {
        return currency.Value;
    }

    public override string ToString()
    {
        return Value.ToFormattedString();
    }
}
