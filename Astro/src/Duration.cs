using System;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {

/// <summary>
/// Measure of time
/// </summary>
public class Duration : IAddable<Duration>, ISubtractable<Duration> {

    private double value;

    /// <summary>
    /// Total number of days
    /// </summary>
    public double TotalDays => TotalHours / 24;
    /// <summary>
    /// Total number of hours
    /// </summary>
    public double TotalHours => TotalMinutes / 60;
    /// <summary>
    /// Total number of minutes
    /// </summary>
    public double TotalMinutes => TotalSeconds / 60;
    /// <summary>
    /// Total number of seconds
    /// </summary>
    public double TotalSeconds => value;
    /// <summary>
    /// Total number of milliseconds
    /// </summary>
    public double TotalMilliseconds => TotalSeconds * 1000;

    /// <summary>
    /// 0 timespan
    /// </summary>
    public static readonly Duration Zero = new Duration(0);
    /// <summary>
    /// Infinite timespan
    /// </summary>
    public static readonly Duration Infinite = new Duration(double.PositiveInfinity);

    /// <summary>
    /// Check if this timespan is infinite
    /// </summary>
    /// <returns>true if timespan is infinite</returns>
    public bool IsInfinite => double.IsInfinity(value);
    /// <summary>
    /// Return the absolute value of this timespan
    /// </summary>
    /// <returns>timespan that has the same value, but is positive</returns>
    public Duration Abs => new Duration(Math.Abs(this.value));

    private Duration(double seconds) {
        this.value = Math.Abs(seconds);
    }

    public override string ToString() {
        return $"{TotalSeconds}s";
    }

    /// <summary>
    /// Implicitly create durations from C# timespan 
    /// </summary>
    /// <param name="span">c# timespan object</param>
    public static implicit operator Duration(TimeSpan span) {
        return new Duration(span.TotalSeconds);
    }

    public override int GetHashCode() {
        return value.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj is Duration timespan) {
            return this.value == timespan.value;
        } else {
            return base.Equals(obj);
        }
    }

    /// <summary>
    /// Create a duration measured in milliseconds
    /// </summary>
    /// <param name="ms">milliseconds</param>
    /// <returns>duration</returns>
    public static Duration Milliseconds(double ms) {
        return Seconds(ms / 1000);
    }

    /// <summary>
    /// Create a duration measured in seconds
    /// </summary>
    /// <param name="seconds">seconds</param>
    /// <returns>duration</returns>
    public static Duration Seconds(double seconds) {
        return new Duration(seconds);
    }

    /// <summary>
    /// Create a duration measured in minutes
    /// </summary>
    /// <param name="minutes">minutes</param>
    /// <returns>duration</returns>
    public static Duration Minutes(double minutes) {
        return new Duration(minutes * 60);
    }

    /// <summary>
    /// Create a duration measured in hours
    /// </summary>
    /// <param name="hours">hours</param>
    /// <returns>duration</returns>
    public static Duration Hours(double hours) {
        return new Duration(hours * 3600);
    }

    /// <summary>
    /// Add two durations
    /// </summary>
    /// <param name="other">other duration</param>
    /// <returns>sum of both durations</returns>
    public Duration Add(Duration other) {
        return new Duration(this.value + other.value);
    }

    public static Duration operator + (Duration a, Duration b) {
        return a.Add(b);
    }

    /// <summary>
    /// Subtract two durations
    /// </summary>
    /// <param name="other">other duration</param>
    /// <returns>difference of both durations</returns>
    public Duration Subtract(Duration other) {
        return new Duration(this.value - other.value);
    }

    public static Duration operator - (Duration a, Duration b) {
        return a.Subtract(b);
    }

    public static bool operator > (Duration a, Duration b) {
        return a.value > b.value;
    }

    public static bool operator < (Duration a, Duration b) {
        return a.value < b.value;
    }
}

}