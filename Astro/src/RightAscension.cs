using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Right Ascension angular measure
/// </summary>
public class RightAscension : AngularMeasure {

    /// <summary>
    /// Maximum value for a RightAscension
    /// </summary>
    public static readonly RightAscension Max = new RightAscension(24);
    /// <summary>
    /// Minimum value for a RightAscension
    /// </summary>
    public static readonly RightAscension Min = new RightAscension(0);

    private double value;

    /// <summary>
	/// Whole angle hours in the measure
	/// </summary>
	/// <returns>angle hours</returns>
    public int Hours => (int)value;
    private double hoursRemaining => value - Hours;
    /// <summary>
	/// Whole angle minutes in the measure
	/// </summary>
	/// <returns>angle minutes</returns>
    public int Minutes => (int)(hoursRemaining * 60);
    private double minutesRemaining => (hoursRemaining * 60) - Minutes;
    /// <summary>
	/// Whole angle seconds in the measure
	/// </summary>
	/// <returns>angle seconds</returns>
    public double Seconds => minutesRemaining * 60;

    /// <summary>
	/// Value of this measure expressed in whole and fractional hour angles
	/// </summary>
    public double TotalHours => value;
    /// <summary>
	/// Value of this measure expressed in whole and fractional minute angles
	/// </summary>
    public double TotalMinutes => Hours * 60;
    /// <summary>
	/// Value of this measure expressed in whole and fractional second angles
	/// </summary>
    public double TotalSeconds => Hours * 3600;

    public override double TotalDegrees => TotalHours * 15; // 1 hour angle is 15 degrees

    /// <summary>
    /// Create RA with the given fractional hour angles
    /// </summary>
    /// <param name="hours">hour angles</param>
    public RightAscension(double hours) {
        this.value = hours.Wrap(0, 24);
    }

    /// <summary>
    /// Create RA with the given hours and fractional minutes
    /// </summary>
    /// <param name="hours">hour angles</param>
    /// <param name="minutes">minute angles</param>
    public RightAscension(int hours, double minutes) : this(hours + minutes / 60) {}

    /// <summary>
    /// Create RA with the given hours, minutes, and seconds
    /// </summary>
    /// <param name="hours">hour angles</param>
    /// <param name="minutes">minutes</param>
    /// <param name="seconds">seconds</param>
    public RightAscension(int hours, int minutes, double seconds) : this(hours + minutes / 60 + seconds / 3600) {}

    /// <summary>
    /// Add hour angles to this angle
    /// </summary>
    /// <param name="hours">hour angles</param>
    /// <returns>new Right Ascension</returns>
    public RightAscension AddHours(double hours) {
        return new RightAscension(this.value + hours);
    }

    /// <summary>
    /// Add minutes to this angle
    /// </summary>
    /// <param name="minutes">minutes</param>
    /// <returns>new Right Ascension</returns>
    public RightAscension AddMinutes(double minutes) {
        return new RightAscension(this.value + minutes / 60);
    }

    /// <summary>
    /// Add seconds to this angle
    /// </summary>
    /// <param name="seconds">seconds</param>
    /// <returns>new Right Ascension</returns>
    public RightAscension AddSeconds(double seconds) {
        return new RightAscension(this.value + seconds / 3600);
    }

    public override string ToString() {
        return $"{Hours}:{Minutes}:{Seconds}";
    }

}


}