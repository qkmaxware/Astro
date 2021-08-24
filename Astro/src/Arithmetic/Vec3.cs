using System;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Arithmetic {

public static class Vec3Extensions {

    /// <summary>
    /// Strip the units off off a vector returning only a vector of unitless values
    /// </summary>
    /// <param name="vector">vector with associated units</param>
    /// <typeparam name="T">type of vector components</typeparam>
    /// <returns>numeric vector</returns>
    public static Vec3<Scientific> StripUnits<T>(this Vec3<T> vector) where T:IConvertable<Scientific>, INumeric<T> {
        return vector.Map(component => { Scientific value; component.Convert(out value); return value; });
    }

    /// <summary>
    /// Scale a vector by the given amount
    /// </summary>
    /// <param name="vector">vector</param>
    /// <param name="scalar">scalar</param>
    /// <typeparam name="T">vector type</typeparam>
    /// <returns>scaled vector</returns>
    public static Vec3<T> ScaleBy<T>(this Vec3<T> vector, Scientific scalar) where T:IScalable<Scientific, T>, INumeric<T> {
        return vector.Map(component => component.ScaleBy(scalar));
    }

    /// <summary>
    /// Rotate the given vector by a quaternion
    /// </summary>
    /// <param name="quat"></param>
    /// <returns></returns>
    public static Vec3<T> Rotate<T>(this Vec3<T> vector, Quat rotation) where T:IScalable<Scientific, T>, INumeric<T> {
        double num12 = rotation.X + rotation.X;
        double num2 = rotation.Y + rotation.Y;
        double num = rotation.Z + rotation.Z;
        double num11 = rotation.W * num12;
        double num10 = rotation.W * num2;
        double num9 = rotation.W * num;
        double num8 = rotation.X * num12;
        double num7 = rotation.X * num2;
        double num6 = rotation.X * num;
        double num5 = rotation.Y * num2;
        double num4 = rotation.Y * num;
        double num3 = rotation.Z * num;
        
        var num15 = ((vector.X.ScaleBy((1f - num5) - num3)).Add(vector.Y.ScaleBy(num7 - num9))).Add(vector.Z.ScaleBy(num6 + num10));
        var num14 = ((vector.X.ScaleBy(num7 + num9)).Add(vector.Y.ScaleBy((1f - num8) - num3))).Add(vector.Z.ScaleBy(num4 - num11));
        var num13 = ((vector.X.ScaleBy(num6 - num10)).Add(vector.Y.ScaleBy(num4 + num11))).Add(vector.Z.ScaleBy((1f - num8) - num5));

        return new Vec3<T>(num15, num14, num13);
    }

}

}