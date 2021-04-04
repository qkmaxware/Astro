using System;

namespace Qkmaxware.Astro.Arithmetic {

/// <summary>
/// Vector with numeric properties of no specific meaning 
/// </summary>
public class Vec3 : Vec3<Real> {
    public static readonly Vec3 Zero = new Vec3(0,0,0);
    public static readonly Vec3 One = new Vec3(1,1,1);
    public static readonly Vec3 I = new Vec3(1,0,0);
    public static readonly Vec3 J = new Vec3(0,1,0);
    public static readonly Vec3 K = new Vec3(0,0,1);

    public Vec3() : base(0,0,0) {}
    public Vec3(Real x, Real y, Real z) : base(x, y, z) {}

    public static Vec3 operator - (Vec3 vec) {
        return new Vec3(-vec.X.Value,-vec.Y.Value,-vec.Z.Value);
    }
}

/// <summary>
/// Abstract vector of 3 dimensions
/// </summary>
/// <typeparam name="T">quantity type for each axis</typeparam>
public class Vec3<T> where T:IAddable<T>, ISubtractable<T>, IMultiplyable<T>, IDividable<T>, IScaleable<T>, ISqrt<T>  {
    /// <summary>
    /// X Coordinate
    /// </summary>
    public T X {get; private set;}
    /// <summary>
    /// Y Coordinate
    /// </summary>
    public T Y {get; private set;}
    /// <summary>
    /// Z Coordinate
    /// </summary>
    public T Z {get; private set;}

    /// <summary>
    /// Squared length of the vector
    /// </summary>    
    public T SqrLength {
        get {
            var xx = X.Multiply(X);
            var yy = Y.Multiply(Y);
            var zz = Z.Multiply(Z);
            return xx.Add(yy).Add(zz);
        }
    }
    /// <summary>
    /// Length of the vector
    /// </summary>
    public T Length => SqrLength.Sqrt();

    /// <summary>
    /// Get a vector of unit length in the same direction
    /// </summary>
    public Vec3<T> Normalized {
        get {
            var mag = this.Length;
            return this / mag;
        }
    }

    /// <summary>
    /// Get a vector pointed in the opposite direction
    /// </summary>
    public Vec3<T> Flipped => -1 * this;

    /// <summary>
    /// Create a vector from components
    /// </summary>
    /// <param name="x">x component</param>
    /// <param name="y">y component</param>
    /// <param name="z">z component</param>
    public Vec3(T x, T y, T z) {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    /// <summary>
    /// Create a vector by copying the components of another
    /// </summary>
    /// <param name="other">other vector</param>
    public Vec3(Vec3<T> other) {
        this.X = other.X;
        this.Y = other.Y;
        this.Z = other.Z;
    }

    /// <summary>
    /// Dot product of two vectors
    /// </summary>
    /// <param name="lhs">first vector</param>
    /// <param name="rhs">second vector</param>
    /// <returns>dot product</returns>
    public static T Dot(Vec3<T> lhs, Vec3<T> rhs) {
        var xx = lhs.X.Multiply(rhs.X);
        var yy = lhs.Y.Multiply(rhs.Y);
        var zz = lhs.Z.Multiply(rhs.Z);

        return xx.Add(yy).Add(zz);
    }

    /// <summary>
    /// Cross product of two vectors
    /// </summary>
    /// <param name="lhs">first vector</param>
    /// <param name="rhs">second vector</param>
    /// <returns>cross product</returns>
    public static Vec3<T> Cross(Vec3<T> lhs, Vec3<T> rhs) {
        var a2b3 = lhs.Y.Multiply(rhs.Z);
        var a3b2 = lhs.Z.Multiply(rhs.Y);
        var a1b3 = lhs.X.Multiply(rhs.Z);
        var a3b1 = lhs.Z.Multiply(rhs.X);
        var a1b2 = lhs.X.Multiply(rhs.Y);
        var a2b1 = lhs.Y.Multiply(rhs.X);

        var i = a2b3.Subtract(a3b2);
        var j = a3b1.Subtract(a1b3);
        var k = a1b2.Subtract(a2b1);

        return new Vec3<T>(
            i,
            j,
            k
        );
    }
    /// <summary>
    /// Distance between two vectors
    /// </summary>
    /// <param name="lhs">first vector</param>
    /// <param name="rhs">second vector</param>
    /// <returns>distance</returns>
    public static T Distance (Vec3<T> lhs, Vec3<T> rhs) {
        return (rhs - lhs).Length;
    }
    /// <summary>
    /// Squared distance between two vectors
    /// </summary>
    /// <param name="lhs">first vector</param>
    /// <param name="rhs">second vector</param>
    /// <returns>squared distance</returns>
    public static T SqrDistance(Vec3<T> lhs, Vec3<T> rhs) {
        return (rhs - lhs).SqrLength;
    }

    /// <summary>
    /// Rotate the given vector by a quaternion
    /// </summary>
    /// <param name="quat"></param>
    /// <returns></returns>
    public Vec3<T> Rotate(Quat rotation) {
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
        
        T num15 = ((this.X.Scale((1f - num5) - num3)).Add(this.Y.Scale(num7 - num9))).Add(this.Z.Scale(num6 + num10));
        T num14 = ((this.X.Scale(num7 + num9)).Add(this.Y.Scale((1f - num8) - num3))).Add(this.Z.Scale(num4 - num11));
        T num13 = ((this.X.Scale(num6 - num10)).Add(this.Y.Scale(num4 + num11))).Add(this.Z.Scale((1f - num8) - num5));

        return new Vec3<T>(num15, num14, num13);
    }

    public static Vec3<T> operator + (Vec3<T> lhs, Vec3<T> rhs) {
        return new Vec3<T>(
            lhs.X.Add(rhs.X),
            lhs.Y.Add(rhs.Y),
            lhs.Z.Add(rhs.Z)
        );
    } 

    public static Vec3<T> operator - (Vec3<T> lhs, Vec3<T> rhs) {
        return new Vec3<T>(
            lhs.X.Subtract(rhs.X),
            lhs.Y.Subtract(rhs.Y),
            lhs.Z.Subtract(rhs.Z)
        );
    } 

    public static Vec3<T> operator * (T lhs, Vec3<T> rhs) {
        return new Vec3<T>(
            lhs.Multiply(rhs.X),
            lhs.Multiply(rhs.Y),
            lhs.Multiply(rhs.Z)
        );
    }

    public static Vec3<T> operator * (Vec3<T> lhs, T rhs) {
        return new Vec3<T>(
            lhs.X.Multiply(rhs),
            lhs.Y.Multiply(rhs),
            lhs.Z.Multiply(rhs)
        );
    }

    public static Vec3<T> operator * (double lhs, Vec3<T> rhs) {
        return new Vec3<T>(
            rhs.X.Scale(lhs),
            rhs.Y.Scale(lhs),
            rhs.Z.Scale(lhs)
        );
    }

    public static Vec3<T> operator * (Vec3<T> lhs, double rhs) {
        return new Vec3<T>(
            lhs.X.Scale(rhs),
            lhs.Y.Scale(rhs),
            lhs.Z.Scale(rhs)
        );
    }

    public static Vec3<T> operator * (Quat rotation, Vec3<T> rhs) {
        return rhs.Rotate(rotation);
    }

    public static Vec3<T> operator / (Vec3<T> lhs, double rhs) {
        return new Vec3<T>(
            lhs.X.Scale(1d/rhs),
            lhs.Y.Scale(1d/rhs),
            lhs.Z.Scale(1d/rhs)
        );
    }

    public static Vec3<T> operator / (Vec3<T> lhs, T rhs) {
        return new Vec3<T>(
            lhs.X.Divide(rhs),
            lhs.Y.Divide(rhs),
            lhs.Z.Divide(rhs)
        );
    }

    /// <summary>
    /// Deconstruct vector to tuple
    /// </summary>
    /// <param name="x">x value</param>
    /// <param name="y">y value</param>
    /// <param name="z">z value</param>
    public void Deconstruct(out T x, out T y, out T z) {
        x = this.X;
        y = this.Y;
        z = this.Z;
    }

    /// <summary>
    /// Convert vector from one type to another
    /// </summary>
    /// <param name="converter">type conversion function</param>
    /// <typeparam name="U">new type</typeparam>
    /// <returns>new vector of the appropriate type</returns>
    public Vec3<U> Convert<U>(Func<T, U> converter) where U:IAddable<U>, ISubtractable<U>, IMultiplyable<U>, IDividable<U>, IScaleable<U>, ISqrt<U> {
        return new Vec3<U>(
            converter(this.X),
            converter(this.Y),
            converter(this.Z)
        );
    }

    public override bool Equals(object obj) {
        if (obj is Vec3<T> vec) {
            return this.X.Equals(vec.X) && this.Y.Equals(vec.Y) && this.Z.Equals(vec.Z);
        } else {
            return base.Equals(obj);
        }
    }

    public override int GetHashCode(){
        return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
    }

    public override string ToString() {
        return $"(x:{X},y:{Y},z:{Z})";
    }    
    
}

}