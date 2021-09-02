using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro {

/// <summary>
/// Duration extensions and constructors for astronomical times
/// </summary>
public static class AstronomicalDuration {

    private static Scientific JulianDays2Seconds = new Scientific(864.00, 2);
    
    /// <summary>
    /// Create a duration measured in julian days
    /// </summary>
    /// <param name="days">number of days</param>
    /// <returns>duration</returns>
    public static Duration JulianDays(Scientific days) {
        return Duration.Seconds(days * JulianDays2Seconds);
    }
    
    /// <summary>
    /// Get value of a duration in julian days
    /// </summary>
    /// <param name="duration">the duration</param>
    /// <returns>the number of julian days represented by the duration</returns>
    public static Scientific TotalJulianDays(this Duration duration) {
        return duration.TotalSeconds() / JulianDays2Seconds;
    }

    // 23 hours 56 minutes and 4.1 seconds
    // 23 hours + 3360 seconds + 4.1 seconds
    // 82800 seconds + 3360 seconds + 4.1 seconds
    private static Scientific SiderealDays2Seconds = 86164.1;

    /// <summary>
    /// Create a duration measured in sidereal days
    /// </summary>
    /// <param name="days">number of days</param>
    /// <returns>duration</returns>
    public static Duration SiderealDays(Scientific days) {
        return Duration.Seconds(days * SiderealDays2Seconds);
    }

    /// <summary>
    /// Get value of a duration in sidereal days
    /// </summary>
    /// <param name="duration">the duration</param>
    /// <returns>the number of sidereal days represented by the duration</returns>
    public static Scientific TotalSiderealDays(this Duration duration) {
        return duration.TotalSeconds() / SiderealDays2Seconds;
    }

}

}