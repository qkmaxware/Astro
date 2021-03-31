using System;
using Qkmaxware.Astro.Arithmetic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Qkmaxware.Astro {

/// <summary>
/// Json converter for angular quantities
/// </summary>
public class AngleJsonConverter : QuantityJsonConverter<Angle> {
	public override string GetSuffix() => "deg";
    public override Angle ParseQuantity(double quant) => Angle.Degrees(quant);
    public override double GetQuantity(Angle value) => value.TotalDegrees;
}
	
/// </summary>
/// Angluar measurement
/// </summary>
[JsonConverter(typeof(AngleJsonConverter))]
public class Angle : IArithmeticValue<Angle> {
	private double value; // internally stored as degrees
	private double magnitude => Math.Abs(value);
	private static double deg2rad =  Math.PI / 180.0;
	private static double rad2deg = 180.0 / Math.PI;

	/// <summary>
    /// Static instance representing a zero angle
    /// </summary>
    public static readonly Angle Zero = new Angle(0);

	# region Value

	/// <summary>
	/// Value of this measure expressed in whole and fractional degrees
	/// </summary>
	public double TotalDegrees => value;
	/// <summary>
	/// Value of this measure expressed in radians
	/// </summary>
	public double TotalRadians => TotalDegrees * deg2rad;
    /// <summary>
	/// Value of this measure expressed in whole and fractional minutes
	/// </summary>
	public double TotalArcMinutes => TotalDegrees * 60;
    /// <summary>
	/// Value of this measure expressed in whole and fractional seconds
	/// </summary>
	public double TotalArcSeconds => TotalDegrees * 3600;

	/// <summary>
	/// Value of this measure expressed in whole and fractional hour angles
	/// </summary>
    public double TotalHours => TotalDegrees / 15d;
    /// <summary>
	/// Value of this measure expressed in whole and fractional minute angles
	/// </summary>
    public double TotalMinutes => TotalHours * 60;
    /// <summary>
	/// Value of this measure expressed in whole and fractional second angles
	/// </summary>
    public double TotalSeconds => TotalHours * 3600;

	/// <summary>
	/// Whole angle hours in the measure
	/// </summary>
	/// <returns>angle hours</returns>
    public int WholeHours => (int)TotalHours;
    private double hoursRemaining => TotalHours - WholeHours;
    /// <summary>
	/// Whole angle minutes in the measure
	/// </summary>
	/// <returns>angle minutes</returns>
    public int WholeMinutes => (int)(hoursRemaining * 60);
    private double minutesRemaining => (hoursRemaining * 60) - WholeMinutes;
    /// <summary>
	/// Whole angle seconds in the measure
	/// </summary>
	/// <returns>angle seconds</returns>
    public double WholeSeconds => minutesRemaining * 60;

	/// <summary>
	/// Whole degrees in the measure
	/// </summary>
	/// <returns>degrees</returns>
	public int WholeDegrees => (int)magnitude;
    /// <summary>
    /// Whole minutes in the measure
    /// </summary>
    /// <returns>minutes</returns>
	public int WholeArcMinutes => (int)((magnitude - WholeDegrees) * 60);
    /// <summary>
    /// Whole seconds in the measure
    /// </summary>
    /// <returns>seconds</returns>
	public double WholeArcSeconds => (magnitude - WholeDegrees - WholeArcMinutes/60.0) * 3600;

	/// <summary>
    /// Coordinate sign
    /// </summary>
    /// <returns>sign of the angular measure</returns>
	public int Sign => Math.Sign(TotalDegrees);

	#endregion

	private Angle(double degrees) {
		this.value = degrees;
	}

	/// <summary>
    /// String representation of an angular coordinate
    /// </summary>
    /// <returns>string</returns>
	public override string ToString() {
		return ToDecimalString();
	}

	/// <summary>
    /// String representation of an angular measure using degrees, minutes, and seconds
    /// </summary>
    /// <returns>string</returns>
	public string ToDecimalString() {
		return (Sign >= 0 ? "+" : "-") + $"{WholeDegrees}°{WholeArcMinutes}'{WholeArcSeconds}\"";
	}

	/// <summary>
    /// String representation of an angular measure using hours, minutes, and seconds
    /// </summary>
    /// <returns>string</returns>
	public string ToHourString() {
        return (Sign >= 0 ? string.Empty : "-") + $"{WholeHours}:{WholeMinutes}:{WholeSeconds}";
	}

	/// <summary>
	/// Create an angle measured in degrees
	/// </summary>
	/// <param name="degrees">degrees</param>
	/// <returns>angle</returns>
	public static Angle Degrees(double degrees) {
		return new Angle(degrees);
	}

	/// <summary>
	/// Create an angle measured in degrees and arc minutes
	/// </summary>
	/// <param name="degrees">degrees</param>
	/// <param name="minutes">arc minutes</param>
	/// <returns>angle</returns>
	public static Angle Degrees (int degrees, double minutes) {
		return Degrees(degrees + minutes / 60.0);
	}

	/// <summary>
	/// Create an angle measured in degrees, arc minutes, and arc seconds
	/// </summary>
	/// <param name="degrees">degrees</param>
	/// <param name="minutes">arc minutes</param>
	/// <param name="seconds">arc seconds</param>
	/// <returns>angle</returns>
	public static Angle Degrees (int degrees, int minutes, double seconds) {
		return Degrees(degrees + minutes / 60.0 + seconds / 3600.0);
	}

	/// <summary>
	/// Create an angle measured in turns
	/// </summary>
	/// <param name="turns">number of turns</param>
	/// <returns>angle</returns>
	public static Angle Turns (double turns) {
		return new Angle(360 * turns);
	}

	/// <summary>
	/// Create an angle measured in gradians
	/// </summary>
	/// <param name="grad">gradians</param>
	/// <returns>angle</returns>
	public static Angle Gradians (double grad) {
		return new Angle(0.9 * grad); // 1 grad = 0.9 deg
	}

	/// <summary>
	/// Create an angle measured in radians
	/// </summary>
	/// <param name="rads">radians</param>
	/// <returns>angle</returns>
	public static Angle Radians(double rads) {
		return new Angle(rads * rad2deg);
	}

	/// <summary>
	/// Create an angle measured in hour angles
	/// </summary>
	/// <param name="hours">hour angles</param>
	/// <returns>angle</returns>
	public static Angle Hours (double hours) {
		return new Angle(hours * 15);
	}

	/// <summary>
	/// Create an angle measured in hours and decimal minutes
	/// </summary>
	/// <param name="hours">hour angles</param>
	/// <param name="minutes">minute angles</param>
	/// <returns>angle</returns>
	public static Angle Hours (int hours, double minutes) {
		return Hours(hours + minutes / 60);
	}

	/// <summary>
	/// Create an angle measured in hours, minutes, and seconds
	/// </summary>
	/// <param name="hours">hour angles</param>
	/// <param name="minutes">minute angles</param>
	/// <param name="seconds">seconds</param>
	/// <returns>angle</returns>
	public static Angle Hours(int hours, int minutes, double seconds) {
		return Hours(hours + minutes / 60 + seconds / 3600);
	}

	/// <summary>
	/// Add two angular values
	/// </summary>
	/// <param name="other">other angle</param>
	/// <returns>new angle</returns>
	public Angle Add(Angle other) {
		return new Angle(this.value + other.value);
	}

	public static Angle operator + (Angle lhs, Angle rhs) {
		return new Angle(lhs.value + rhs.value);
	}

	/// <summary>
	/// Subtract two angular values
	/// </summary>
	/// <param name="other">other angle</param>
	/// <returns>new angle</returns>
	public Angle Subtract(Angle other) {
		return new Angle(this.value - other.value);
	}

	public static Angle operator - (Angle lhs, Angle rhs) {
		return new Angle(lhs.value - rhs.value);
	}

	/// <summary>
	/// Multiply two angular values
	/// </summary>
	/// <param name="other">other angle</param>
	/// <returns>new angle</returns>
	public Angle Multiply(Angle other) {
		return new Angle(this.value * other.value);
	}

	/// <summary>
	/// Divide two angles
	/// </summary>
	/// <param name="other">other angle</param>
	/// <returns>new angle</returns>
	public Angle Divide(Angle other) {
		return new Angle(this.value / other.value);
	}

	/// <summary>
	/// Square root of the angle
	/// </summary>
	/// <returns>new angle</returns>
	public Angle Sqrt() {
		return new Angle(System.Math.Sqrt(this.value));
	}

	/// <summary>
	/// Scale this angle by a scalar value
	/// </summary>
	/// <param name="scale">scalar value</param>
	/// <returns>new angle</returns>
	public Angle Scale(double scale) {
		return new Angle(this.value * scale);
	}

	public override bool Equals(object obj) {
        if (obj is Angle real) {
            return this.value == real.value;
        } else 
            return base.Equals(obj);
    }

    public override int GetHashCode(){
        return value.GetHashCode();
    }
}

}