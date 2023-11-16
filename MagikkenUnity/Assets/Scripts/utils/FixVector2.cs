using System;
using FixMath.NET;
using UnityEngine;

[Serializable]
public struct FixVector2 : IEquatable<FixVector2>
{
    public Fix64 x;
    public Fix64 y;

    public FixVector2(Fix64 x, Fix64 y)
    {
        this.x = x;
        this.y = y;
    }

    #region Operator overrides
    public static FixVector2 operator +(FixVector2 v1, FixVector2 v2)
    {
        Fix64 newX = v1.x + v2.x;
        Fix64 newY = v1.y + v2.y;
        return new FixVector2(newX, newY);
    }

    public static FixVector2 operator -(FixVector2 v1, FixVector2 v2)
    {
        Fix64 newX = v1.x - v2.x;
        Fix64 newY = v1.y - v2.y;
        return new FixVector2(newX, newY);
    }

    public static FixVector2 operator *(FixVector2 v1, FixVector2 v2)
    {
        Fix64 newX = v1.x * v2.x;
        Fix64 newY = v1.y * v2.y;
        return new FixVector2(newX, newY);
    }

    public static FixVector2 operator *(FixVector2 v1, Fix64 f1)
    {
        Fix64 newX = v1.x * f1;
        Fix64 newY = v1.y * f1;
        return new FixVector2(newX, newY);
    }

    public static FixVector2 operator /(FixVector2 v1, FixVector2 v2)
    {
        Fix64 newX = v1.x / v2.x;
        Fix64 newY = v1.y / v2.y;
        return new FixVector2(newX, newY);
    }

    public static FixVector2 operator /(FixVector2 v1, Fix64 f1)
    {
        Fix64 newX = v1.x / f1;
        Fix64 newY = v1.y / f1;
        return new FixVector2(newX, newY);
    }

    public static bool operator ==(FixVector2 v1, FixVector2 v2)
    {
        bool isEqX = v1.x == v2.x;
        bool isEqY = v1.y == v2.y;
        return (isEqX && isEqY);
    }

    public static bool operator !=(FixVector2 v1, FixVector2 v2)
    {
        bool isEqX = v1.x == v2.x;
        bool isEqY = v1.y == v2.y;
        return (!(isEqX && isEqY));
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector2 && ((FixVector2)obj == this);
    }

    public bool Equals(FixVector2 other)
    {
        return x.Equals(other.x) &&
               y.Equals(other.y);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
    #endregion

    public static explicit operator Vector2(FixVector2 v)
    {
        return new Vector2((float)v.x, (float)v.y);
    }

    /// <summary>
    /// Returns a number indicating the sign of the x component of a FixVector2.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    public static int SignX(FixVector2 v)
    {
        return Fix64.Sign(v.x);
    }

    /// <summary>
    /// Returns a number indicating the sign of the y component of a FixVector2.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    public static int SignY(FixVector2 v)
    {
        return Fix64.Sign(v.y);
    }

    public static Fix64 DotProduct(FixVector2 v1, FixVector2 v2)
    {
        return (v1.x * v2.x) + (v1.y * v2.y);
    }

    public Fix64 Magnitude()
    {
        return Fix64.Sqrt((x * x) + (y * y));
    }

    public Fix64 MagnitudeSquared()
    {
        return ((x * x) + (y * y));
    }

    public FixVector2 Normalized()
    {
        return (this / Magnitude());
    }

    public static readonly FixVector2 Zero = new FixVector2(Fix64.Zero, Fix64.Zero);
    public static readonly FixVector2 UnitX = new FixVector2(Fix64.One, Fix64.Zero);
    public static readonly FixVector2 UnitY = new FixVector2(Fix64.Zero, Fix64.One);
}
