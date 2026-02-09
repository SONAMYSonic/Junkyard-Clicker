using System;
using JunkyardClicker.Core;

// 재화 값 타입 - 음수 불가, 큰 숫자 포맷 지원
public readonly struct Currency
{
    public static readonly Currency Zero = new Currency(0);

    public readonly double Value;

    // 생성자 - 음수 값 방지
    public Currency(double value)
    {
        if (value < 0)
        {
            throw new Exception("Currency 값은 0보다 작을 수 없습니다.");
        }

        Value = value;
    }

    // 덧셈 연산자
    public static Currency operator +(Currency a, Currency b)
    {
        return new Currency(a.Value + b.Value);
    }

    // 뺄셈 연산자
    public static Currency operator -(Currency a, Currency b)
    {
        return new Currency(a.Value - b.Value);
    }

    // 비교 연산자들
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

    // double에서 암시적 변환
    public static implicit operator Currency(double value)
    {
        return new Currency(value);
    }

    // double로 명시적 변환
    public static explicit operator double(Currency currency)
    {
        return currency.Value;
    }

    // 포맷팅된 문자열 반환
    public override string ToString()
    {
        return Value.ToFormattedString();
    }
}
