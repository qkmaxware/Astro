using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Abstract base class for classes representing angular measurements
/// </summary>
public abstract class AngularMeasure {
	/// <summary>
	/// Value of this measure expressed in whole and fractional degrees
	/// </summary>
	public abstract double TotalDegrees {get;}
    /// <summary>
	/// Value of this measure expressed in whole and fractional minutes
	/// </summary>
	public double TotalArcMinutes => TotalDegrees * 60;
    /// <summary>
	/// Value of this measure expressed in whole and fractional seconds
	/// </summary>
	public double TotalArcSeconds => TotalDegrees * 3600;

	private double magnitude => Math.Abs(TotalDegrees);

	/// <summary>
	/// Whole degrees in the measure
	/// </summary>
	/// <returns>degrees</returns>
	public int Degrees => (int)magnitude;
    /// <summary>
    /// Whole minutes in the measure
    /// </summary>
    /// <returns>minutes</returns>
	public int ArcMinutes => (int)((magnitude - Degrees) * 60);
    /// <summary>
    /// Whole seconds in the measure
    /// </summary>
    /// <returns>seconds</returns>
	public double ArcSeconds => (magnitude - Degrees - ArcMinutes/60.0) * 3600;

	/// <summary>
    /// Coordinate sign
    /// </summary>
    /// <returns>sign of the angular measure</returns>
	public int Sign => Math.Sign(TotalDegrees);
}

/// <summary>
/// Declination angular measure
/// </summary>
public class Declination : AngularMeasure {

    private double value;
	
	public override double TotalDegrees => value;

    public Declination(double degrees) {
        this.value = degrees.Wrap(-180, 180);
    }

    /// <summary>
	/// Create a new coordinate in degrees, minutes, and seconds
	/// </summary>
	/// <param name="degrees">whole degrees</param>
	/// <param name="minutes">whole minutes</param>
	/// <param name="seconds">seconds</param>
	public Declination(int degrees, int minutes, double seconds) : this(degrees + minutes / 60.0 + seconds / 3600.0) {}

	/// <summary>
	/// Add the given number of degrees to this coordinate
	/// </summary>
	/// <param name="degrees">degrees</param>
	/// <returns>new angular coordinate</returns>
	public Declination AddDegrees(double degrees) {
		return new Declination(this.value + degrees);
	}

	/// <summary>
	/// Add the given number of minutes to this coordinate
	/// </summary>
	/// <param name="minutes">minutes</param>
	/// <returns>new angular coordinate</returns>
	public Declination AddMinutes(double minutes) {
		return new Declination(this.value + minutes / 60.0);
	}
	
	/// <summary>
	/// Add the given number of seconds to this coordinate
	/// </summary>
	/// <param name="seconds">seconds</param>
	/// <returns>new angular coordinate</returns>
	public Declination AddSeconds(double seconds) {
		return new Declination(this.value + seconds / 3600.0);
	}

    /// <summary>
    /// String representation of an angular coordinate
    /// </summary>
    /// <returns>string</returns>
	public override string ToString() {
		return (Sign >= 0 ? "+" : "-") + $"{Degrees}°{ArcMinutes}'{ArcSeconds}\"";
	}

}

}