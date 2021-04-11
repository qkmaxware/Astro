using System;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {

/// <summary>
/// Class representing astronomical distances
/// </summary>
public class Distance : IArithmeticValue<Distance> {

    private double value; //km

    /// <summary>
    /// Total distance in metres
    /// </summary>
    public double TotalMetres => TotalKilometres * 1000d;
    /// <summary>
    /// Total distance in kilometres
    /// </summary>
    public double TotalKilometres => value;
    /// <summary>
    /// Total distance in light years
    /// </summary>
    public double TotalLightYears => TotalKilometres / 9460730472580.800d;
    /// <summary>
    /// Total distance in parsecs
    /// </summary>
    public double TotalParsecs => TotalKilometres / 30856775812800d;

    private Distance (double km) {
        this.value = km;
    }

    public override string ToString() {
        if (value < 1) {
            return string.Format("{0:0.##}", TotalMetres) + "m";
        } else if (value > 9e12) {
            return string.Format("{0:0.##}", TotalLightYears) + "ly";
        } else {
            return string.Format("{0:0.##}", TotalKilometres) + "km";
        }
    }

    /// <summary>
    /// Static instance representing 0 distance
    /// </summary>
    public static readonly Distance Zero = new Distance(0);

    /// <summary>
    /// Create a distance in metres
    /// </summary>
    /// <param name="value">metres</param>
    /// <returns>distance</returns>
    public static Distance Metres(double value) {
        return new Distance(value / 1000d);
    }

    /// <summary>
    /// Create a distance in kilometres
    /// </summary>
    /// <param name="value">kilometres</param>
    /// <returns>distance</returns>
    public static Distance Kilometres(double value) {
        return new Distance(value);
    }

    /// <summary>
    /// Create a distance in Astronomical Units (AU)
    /// </summary>
    /// <param name="value">au</param>
    /// <returns>distance</returns>
    public static Distance AU(double value) {
        return new Distance(value * 149598073);
    }

    /// <summary>
    /// Create a distance in light years
    /// </summary>
    /// <param name="value">light years</param>
    /// <returns>distance</returns>
    public static Distance Lightyears(double value) {
        return new Distance(value * 9460730472580.800d);
    }
    /// <summary>
    /// Create a distance in parsecs
    /// </summary>
    /// <param name="value">parsecs</param>
    /// <returns>distance</returns>
    public static Distance Parsecs(double value) {
        return new Distance(value * 30856775812800d);
    }
    /// <summary>
    /// Create a distance in kiloparsecs
    /// </summary>
    /// <param name="value">kiloparsecs</param>
    /// <returns>distance</returns>
    public static Distance Kiloparsecs(double value) {
        return Parsecs(value * 1000d);
    }
    /// <summary>
    /// Create a distance in megaparsecs
    /// </summary>
    /// <param name="value">megaparsecs</param>
    /// <returns>distance</returns>
    public static Distance Megaparsecs(double value) {
        return Parsecs(value * 1000000d);
    }

    public Distance Add(Distance other) {
        return this + other;
    }

    public Distance Subtract(Distance other) {
        return this - other;
    }

    public Distance Multiply(Distance other) {
        return this * other;
    }

    public Distance Divide(Distance other) {
        return this / other;
    }

    public Distance Sqrt() {
        return new Distance(Math.Sqrt(this.value));
    }

    /// <summary>
	/// Scale this distance by a scalar value
	/// </summary>
	/// <param name="scale">scalar value</param>
	/// <returns>new distance</returns>
	public Distance Scale(double scale) {
		return new Distance(this.value * scale);
	}

    public static Distance operator + (Distance a, Distance b) {
        return new Distance(a.value + b .value);
    }

    public static Distance operator - (Distance a, Distance b) {
        return new Distance(a.value - b .value);
    }

    public static Distance operator * (Distance a, Distance b) {
        return new Distance(a.value * b .value);
    }

    public static Distance operator * (Distance a, double scale) {
        return new Distance(a.value * scale);
    }

    public static Distance operator * (double scale, Distance a) {
        return new Distance(a.value * scale);
    }

    public static Distance operator / (Distance a, Distance b) {
        return new Distance(a.value / b .value);
    }

    public override bool Equals(object obj) {
        if (obj is Distance real) {
            return this.value == real.value;
        } else 
            return base.Equals(obj);
    }

    public override int GetHashCode(){
        return value.GetHashCode();
    }
}

}