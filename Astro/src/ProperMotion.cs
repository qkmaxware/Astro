using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Proper motion of an astronomical object
/// </summary>
public class ProperMotion {
    /// <summary>
    /// Rate of change of an object's right ascension angle
    /// </summary>
    public RateOfChange<Angle> RightAscensionRate {get; private set;}
    /// <summary>
    /// Rate of change of an object's declination angle
    /// </summary>
    public RateOfChange<Angle> DeclinationRate    {get; private set;}
    /// <summary>
    /// Create a new proper motion object
    /// </summary>
    /// <param name="raRate">rate of change for right ascension</param>
    /// <param name="decRate">rate of change for declination</param>
    public ProperMotion(RateOfChange<Angle> raRate, RateOfChange<Angle> decRate) {
        this.RightAscensionRate = raRate;
        this.DeclinationRate = decRate;
    }
}

}