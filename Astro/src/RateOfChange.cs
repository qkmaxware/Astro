using System;
using Qkmaxware.Measurement;

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
    public Duration Duration  {get; private set;}
    /// <summary>
    /// Create a new rate of change
    /// </summary>
    /// <param name="amount">amount of change</param>
    /// <param name="duration">duration over which change takes place</param>
    public RateOfChange (T amount, TimeSpan duration) {
        this.Amount = amount;
        this.Duration = Duration.Seconds(duration.TotalSeconds);
    }
    /// <summary>
    /// Create a new rate of change
    /// </summary>
    /// <param name="amount">amount of change</param>
    /// <param name="duration">duration over which change takes place</param>
    public RateOfChange (T amount, Duration duration) {
        this.Amount = amount;
        this.Duration = duration;
    }
}

}