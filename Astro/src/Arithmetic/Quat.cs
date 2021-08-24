using System;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro.Arithmetic {

/// <summary>
/// A rotation represented as a 4D vector or quaternion
/// </summary>
public class Quat {
    public static readonly Quat Identity = new Quat(0, 0, 0, 1);

    // ---------------------------------------------------------------------
    // 1D Subsets
    // ---------------------------------------------------------------------

    /// <summary>
    /// X Component 
    /// </summary>
    public double X {get; private set;}
    /// <summary>
    /// Y Component 
    /// </summary>
    public double Y {get; private set;}
    /// <summary>
    /// Z Component 
    /// </summary>
    public double Z {get; private set;}
    /// <summary>
    /// W Component 
    /// </summary>
    public double W {get; private set;}

    /// <summary>
    /// Create a new quaternion from the given components
    /// </summary>
    /// <param name="x">x component</param>
    /// <param name="y">y component</param>
    /// <param name="z">z component</param>
    /// <param name="w">w component</param>
    public Quat(double x = 0, double y = 0, double z = 0, double w = 1) {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    /// <summary>
    /// Copy an existing quaternion
    /// </summary>
    /// <param name="other">quaternion to copy</param>
    public Quat (Quat other) : this (other.X, other.Y, other.Z, other.W) {}

    /// <summary>
    /// Conjugate of this quaternion
    /// </summary>
    /// <returns>conjugate quaternion</returns>
    public Quat Conjugate => new Quat(-this.X, -this.Y, -this.Z, this.W);    

    /// <summary>
    /// Length of the quaternion vector
    /// </summary>
    public double Length => System.Math.Sqrt(SqrLength);

    /// <summary>
    /// Squared length of the quaternion vector
    /// </summary>
    public double SqrLength => (X * X + Y * Y + Z * Z + W * W);

    /// <summary>
    /// Quaternion vector in the same direction but of unit length
    /// </summary>
    public Quat Normalized {
        get {
            double length = Length;
            if (length == 0) {
                throw new DivideByZeroException();
            }
            double invLength = 1 / length;
            return new Quat(X * invLength, Y * invLength, Z * invLength, W * invLength);
        }
    }

    /// <summary>
    /// Quaternion vector with the absolute value of all components
    /// </summary>
    public Quat Abs => new Quat(System.Math.Abs(X), System.Math.Abs(Y), System.Math.Abs(Z), System.Math.Abs(W));

    /// <summary>
    /// Quaternion vector flipped in the opposite direction
    /// </summary>
    public Quat Flipped => -1 * this;

    /// <summary>
    /// Dot product of two quaternion vectors
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <returns>vector dot product</returns>
    public static double Dot(Quat a, Quat b) {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    /// <summary>
    /// Angle between two quaternions
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <returns>angle</returns>
    public static double Angle (Quat a, Quat b) {
        var d = Dot(a, b);
        return Math.Acos(Math.Min(Math.Abs(d), 1) * 2);
    }

    /// <summary>
    /// Linearly interpolate between two quaternions
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <param name="t">interpolation factor</param>
    /// <returns>interpolated quaternion</returns>
    public static Quat Lerp(Quat a, Quat b, double t) {
        return (1 - t) * a + t * b;
    }

    /// <summary>
    /// Spherically interpolate between two quaternions
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <param name="t">interpolation factor</param>
    /// <returns>interpolated quaternion</returns>
    public static Quat Slerp(Quat a, Quat b, double t) {
        a = a.Normalized;
        b = b.Normalized;

        var dot = Dot(a, b);

        if (dot < 0) {
            b = -1 * b;
            dot = -dot;
        }

        double theta0 = Math.Acos(dot);
        double theta = theta0 * t;
        double sin_theta = Math.Sin(theta);
        double sin_theta0 = Math.Sin(theta0);

        double s0 = Math.Cos(theta) - dot * sin_theta / sin_theta0;
        double s1 = sin_theta / sin_theta0;

        return (s0 * a) + (s1 * b);
    }

    /// <summary>
    /// Create a quaternion from yaw-pitch-roll rotation angles
    /// </summary>
    /// <param name="yaw">yaw angle</param>
    /// <param name="pitch">pitch angle</param>
    /// <param name="roll">roll angle</param>
    /// <returns>quaternion</returns>
    public static Quat YawPitchRoll(double yaw, double pitch, double roll) {
        double cy = Math.Cos(yaw * 0.5);
        double sy = Math.Sin(yaw * 0.5);
        double cp = Math.Cos(pitch * 0.5);
        double sp = Math.Sin(pitch * 0.5);
        double cr = Math.Cos(roll * 0.5);
        double sr = Math.Sin(roll * 0.5);

        return new Quat(
            x: sr * cp * cy - cr * sp * sy,
            y: cr * sp * cy + sr * cp * sy,
            z: cr * cp * sy - sr * sp * cy,
            w: cr * cp * cy + sr * sp * sy
        );
    }

    /// <summary>
    /// Create a quaternion from yaw-pitch-roll rotation angles
    /// </summary>
    /// <param name="yaw">yaw angle</param>
    /// <param name="pitch">pitch angle</param>
    /// <param name="roll">roll angle</param>
    /// <returns>quaternion</returns>
    public static Quat YawPitchRoll(Angle yaw, Angle pitch, Angle roll) {
        return YawPitchRoll((double)yaw.TotalRadians(), (double)pitch.TotalRadians(), (double)roll.TotalRadians());
    }

    /// <summary>
    /// Sum of two quaternions
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <returns>sum</returns>
    public static Quat operator + (Quat a, Quat b) {
        return new Quat(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

    /// <summary>
    /// Quaternion vector negation
    /// </summary>
    /// <param name="value">first vector</param>
    /// <returns>quaternion with all values negated</returns>
    public static Quat operator - (Quat value) {
        return new Quat(-value.X, -value.Y, -value.Z, -value.W);
    } 

    /// <summary>
    /// Difference of two quaternions
    /// </summary>
    /// <param name="a">first quaternion</param>
    /// <param name="b">second quaternion</param>
    /// <returns>difference</returns>
    public static Quat operator - (Quat a, Quat b) {
        return new Quat(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    }

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="a">scalar value</param>
    /// <param name="b">quaternion</param>
    /// <returns>scalar multiplication</returns>
    public static Quat operator * (double a, Quat b) {
        return new Quat(a * b.X, a * b.Y, a * b.Z, a * b.W);
    }

    /// <summary>
    /// Scalar multiplication
    /// </summary>
    /// <param name="a">quaternion</param>
    /// <param name="b">scalar value</param>
    /// <returns>scalar multiplication</returns>
    public static Quat operator * (Quat a, double b) {
        return new Quat(a.X * b, a.Y * b, a.Z * b, a.W * b);
    }

    /// <summary>
    /// Scalar division
    /// </summary>
    /// <param name="a">quaternion</param>
    /// <param name="b">scalar value</param>
    /// <returns>scalar division</returns>
    public static Quat operator / (Quat a, double b) {
        return new Quat(a.X / b, a.Y / b, a.Z / b, a.W / b);
    }

    /// <summary>
    /// Multiply two quaternions
    /// </summary>
    /// <param name="lhs">first quaternion</param>
    /// <param name="rhs">second quaternion</param>
    /// <returns>pr</returns>
    public static Quat operator * (Quat lhs, Quat rhs) {
        var a = lhs.W;  var e = rhs.W;
        var b = lhs.X;  var f = rhs.X;
        var c = lhs.Y;  var g = rhs.Y;
        var d = lhs.Z;  var h = rhs.Z;

        var w = a*e - b*f - c*g- d*h;
        var x = (b*e + a*f + c*h - d*g);
        var y = (a*g - b*h + c*e + d*f);
        var z = (a*h + b*g - c*f + d*e);

        return new Quat(x, y, z, w);
    }

    /// <summary>
    /// Quaternion division
    /// </summary>
    /// <param name="lhs">first quaternion</param>
    /// <param name="rhs">second quaternion</param>
    /// <returns>division of the first by the second quaternion</returns>
    public static Quat operator / (Quat lhs, Quat rhs) {
        return lhs * (rhs.Conjugate.Normalized);
    }

    /// <summary>
    /// Convert vector to string
    /// </summary>
    /// <returns>string representation of the vector</returns>
    public override string ToString() {
        return string.Format("(x:{0:0.000},y:{1:0.000},z:{2:0.000},w:{3:0.000})", X, Y, Z, W);
    }

    /// <summary>
    /// Vector equality check
    /// </summary>
    /// <param name="obj">object to check against</param>
    /// <returns>true if the object is a vector and the components match</returns>
    public override bool Equals(object obj) {
        return obj switch {
            Quat other => this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W,
            _ => base.Equals(obj)
        };
    }

    public override int GetHashCode() {
        return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();
    }
}

}