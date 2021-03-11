using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Class representing astronomical distances
/// </summary>
public class Distance {

    private double value; //km

    /// <summary>
    /// Total distance in kilometres
    /// </summary>
    public double TotalKilometres => value;
    /// <summary>
    /// Total distance in light years
    /// </summary>
    public double TotalLightYears => value / 9460730472580.800d;
    /// <summary>
    /// Total distance in parsecs
    /// </summary>
    public double TotalParsecs => value / 30856775812800d;

    private Distance (double km) {
        this.value = km;
    }

    public override string ToString() {
        return string.Format("{0:0.##}", TotalLightYears) + "ly";
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

}

}