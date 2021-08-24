using Qkmaxware.Measurement;

namespace Qkmaxware.Astro.Constants {

/// <summary>
/// Mass constants
/// </summary>
public static class Masses {
    /// <summary>
    /// Mass of the sun
    /// </summary>
    /// <returns>mass</returns>
    public static readonly Mass Sun = Mass.Kilograms(new Numbers.Scientific(1.989, 30));
    /// <summary>
    /// Mass of the planet Earth
    /// </summary>
    /// <returns>mass</returns>
    public static readonly Mass Earth = Mass.Kilograms(5.972e24);
}

}