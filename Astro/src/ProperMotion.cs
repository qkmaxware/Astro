using System;

namespace Qkmaxware.Astro {

//  arcseconds per year (symbols: arcsec/yr, as/yr, ″/yr, ″ yr−1) or milliarcseconds per year (symbols: mas/yr, mas yr−1)
/// <summary>
/// Class describing the rate of change of a type over time
/// </summary>
/// <typeparam name="T">type of change</typeparam>
public class RateOfChange<T> {
    /// <summary>
    /// Amount of change over the given duration
    /// </summary>
    /// <value>amount of change</value>
    public T Amount {get; private set;}
    /// <summary>
    /// Duration over which the change takes place
    /// </summary>
    /// <value>duration of change</value>
    public TimeSpan Duration  {get; private set;}
    /// <summary>
    /// Create a new rate of change
    /// </summary>
    /// <param name="amount">amount of change</param>
    /// <param name="duration">duration over which change takes place</param>
    public RateOfChange (T amount, TimeSpan duration) {
        this.Amount = amount;
        this.Duration = duration;
    }
}

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