using Qkmaxware.Numbers;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro {

/// <summary>
/// Length extensions and constructors for astronomical distances and lengths
/// </summary>
public static class AstronomicalLength {
    private static Scientific Au2Km = 149598073;

    /// <summary>
    /// Create a distance in astronomical units
    /// </summary>
    /// <param name="value">astronomical units</param>
    /// <returns>length</returns>
    public static Length AU(Scientific value) {
        return Length.Kilometres(value * Au2Km);
    }

    /// <summary>
    /// Total distance in astronomical units
    /// </summary>
    public static Scientific TotalAU(this Length length) {
        return length.TotalKilometres() / Au2Km;    
    }

    private static Scientific Lightyears2Km = new Scientific(9460730472580800, -3);

    /// <summary>
    /// Create a distance in light years
    /// </summary>
    /// <param name="value">light years</param>
    /// <returns>length</returns>
    public static Length LightYears(Scientific value) {
        return Length.Kilometres(value * Lightyears2Km);
    }

    /// <summary>
    /// Total distance in light years
    /// </summary>
    public static Scientific TotalLightYears(this Length length) {
        return length.TotalKilometres() / Lightyears2Km;
    }

    private static Scientific Parsecs2Km = 30856775812800;

    /// <summary>
    /// Create a distance in parsecs
    /// </summary>
    /// <param name="value">parsecs</param>
    /// <returns>length</returns>
    public static Length Parsecs(Scientific value) {
        return Length.Kilometres(value * Parsecs2Km);
    }

    /// <summary>
    /// Total distance in parsecs
    /// </summary>
    public static Scientific TotalParsecs(this Length length) {
        return length.TotalKilometres() / Parsecs2Km;
    }

    /// <summary>
    /// Create a distance in kiloparsecs
    /// </summary>
    /// <param name="value">kiloparsecs</param>
    /// <returns>length</returns>
    public static Length Kiloparsecs(double value) {
        return Parsecs(value * 1000d);
    }
    /// <summary>
    /// Create a distance in megaparsecs
    /// </summary>
    /// <param name="value">megaparsecs</param>
    /// <returns>length</returns>
    public static Length Megaparsecs(double value) {
        return Parsecs(value * 1000000d);
    }
} 

}