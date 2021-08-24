using Qkmaxware.Numbers;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro {

/// <summary>
/// Mass extensions and constructors for astronomical masses
/// </summary>
public static class AstronomicalMass {

    private static double G = 6.67384e-11;
    
    /// <summary>
    /// Standard gravitational parametre of this mass
    /// </summary>
    /// <param name="mass">mass</param>
    /// <returns>Standard gravitational parametre</returns>
    public static double μ(this Mass mass) => G * (double)mass.TotalKilograms();

    private static Scientific SolarMass2Kg = new Scientific(2, 30);

    /// <summary>
    /// Create a mass measured in solar masses
    /// </summary>
    /// <param name="value">solar masses</param>
    /// <returns>mass</returns>
    public static Mass SolarMasses(Scientific value) {
        return Mass.Kilograms(value * SolarMass2Kg);
    }

    /// <summary>
    /// Get the mass of in solar masses
    /// </summary>
    /// <param name="mass">mass</param>
    /// <returns>solar masses</returns>
    public static Scientific TotalSolarMasses(this Mass mass) {
        return mass.TotalKilograms() / SolarMass2Kg;
    }

}

}