using System;
using FixMath.NET;
using UnityEngine;

public static class Fix64Math
{
    public static FixVector3 DuelToFieldSpace(FixVector2 duelVector, FixVector3 cameraForward, FixVector3 cameraRight)
    {
        FixVector3 result = new FixVector3();

        result.x = duelVector.x;
        result.y = duelVector.y;
        result.z = Fix64.Zero;

        return result;
    }

    public static Fix64 MoveTowards(Fix64 start, Fix64 target, Fix64 step)
    {
        if (start < target)
        {
            if (start + step > target) { return target; }
            else { return start + step; }
        }
        else
        {
            if (start - step < target) { return target; }
            else { return start - step; }
        }
    }

    public static FixVector2 MoveTowards(FixVector2 start, FixVector2 target, Fix64 step)
    {
        return new FixVector2(
            MoveTowards(start.x, target.x, step),
            MoveTowards(start.y, target.y, step)
            );
    }

    public static FixVector3 MoveTowards(FixVector3 start, FixVector3 target, Fix64 step)
    {

        return new FixVector3(
            MoveTowards(start.x, target.x, step),
            MoveTowards(start.y, target.y, step),
            MoveTowards(start.z, target.z, step)
            );
    }
}