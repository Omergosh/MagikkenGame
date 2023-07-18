using System;
using FixMath.NET;

[Serializable]
public struct FixVector3 : IEquatable<FixVector3>
{
    public Fix64 x;
    public Fix64 y;
    public Fix64 z;

    public FixVector3(Fix64 x, Fix64 y, Fix64 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    #region Operator overrides
    public static FixVector3 operator +(FixVector3 v1, FixVector3 v2)
    {
        Fix64 newX = v1.x + v2.x;
        Fix64 newY = v1.y + v2.y;
        Fix64 newZ = v1.z + v2.z;
        return new FixVector3(newX, newY, newZ);
    }

    public static FixVector3 operator -(FixVector3 v1, FixVector3 v2)
    {
        Fix64 newX = v1.x - v2.x;
        Fix64 newY = v1.y - v2.y;
        Fix64 newZ = v1.z - v2.z;
        return new FixVector3(newX, newY, newZ);
    }

    public static FixVector3 operator *(FixVector3 v1, FixVector3 v2)
    {
        Fix64 newX = v1.x * v2.x;
        Fix64 newY = v1.y * v2.y;
        Fix64 newZ = v1.z * v2.z;
        return new FixVector3(newX, newY, newZ);
    }

    public static FixVector3 operator *(FixVector3 v1, Fix64 f1)
    {
        Fix64 newX = v1.x * f1;
        Fix64 newY = v1.y * f1;
        Fix64 newZ = v1.z * f1;
        return new FixVector3(newX, newY, newZ);
    }

    public static FixVector3 operator /(FixVector3 v1, FixVector3 v2)
    {
        Fix64 newX = v1.x / v2.x;
        Fix64 newY = v1.y / v2.y;
        Fix64 newZ = v1.z / v2.z;
        return new FixVector3(newX, newY, newZ);
    }

    public static FixVector3 operator /(FixVector3 v1, Fix64 f1)
    {
        Fix64 newX = v1.x / f1;
        Fix64 newY = v1.y / f1;
        Fix64 newZ = v1.z / f1;
        return new FixVector3(newX, newY, newZ);
    }

    public static bool operator ==(FixVector3 v1, FixVector3 v2)
    {
        bool isEqX = v1.x == v2.x;
        bool isEqY = v1.y == v2.y;
        bool isEqZ = v1.z == v2.z;
        return (isEqX && isEqY && isEqZ);
    }

    public static bool operator !=(FixVector3 v1, FixVector3 v2)
    {
        bool isEqX = v1.x == v2.x;
        bool isEqY = v1.y == v2.y;
        bool isEqZ = v1.z == v2.z;
        return (!(isEqX && isEqY && isEqZ));
    }

    public override bool Equals(object obj)
    {
        return obj is FixVector3 && ((FixVector3)obj == this);
    }

    public bool Equals(FixVector3 other)
    {
        return x.Equals(other.x) &&
               y.Equals(other.y) &&
               z.Equals(other.z);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }
    #endregion

    /// <summary>
    /// Returns a number indicating the sign of the x component of a FixVector3.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    public static int SignX(FixVector3 v)
    {
        return Fix64.Sign(v.x);
    }

    /// <summary>
    /// Returns a number indicating the sign of the y component of a FixVector3.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    public static int SignY(FixVector3 v)
    {
        return Fix64.Sign(v.y);
    }

    /// <summary>
    /// Returns a number indicating the sign of the z component of a FixVector3.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    public static int SignZ(FixVector3 v)
    {
        return Fix64.Sign(v.z);
    }

    public static Fix64 DotProduct(FixVector3 v1, FixVector3 v2)
    {
        return (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
    }

    public static FixVector3 CrossProduct(FixVector3 v1, FixVector3 v2)
    {
        Fix64 newX = (v1.y * v2.z) - (v1.z * v2.y);
        Fix64 newY = (v1.z * v2.x) - (v1.x * v2.z);
        Fix64 newZ = (v1.x * v2.y) - (v1.y * v2.z);
        return new FixVector3(newX, newY, newZ);
    }

    public Fix64 Magnitude()
    {
        return Fix64.Sqrt((x * x) + (y * y) + (z * z));
    }

    public Fix64 MagnitudeSquared()
    {
        return ((x * x) + (y * y) + (z * z));
    }

    public FixVector3 Normalized()
    {
        return (this / Magnitude());
    }

    public static readonly FixVector3 Zero = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    public static readonly FixVector3 UnitX = new FixVector3(Fix64.One, Fix64.Zero, Fix64.Zero);
    public static readonly FixVector3 UnitY = new FixVector3(Fix64.Zero, Fix64.One, Fix64.Zero);
    public static readonly FixVector3 UnitZ = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.One);
}
