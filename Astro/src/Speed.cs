using System;
using System.Text.Json.Serialization;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {

/// <summary>
/// Json converter for distance quantities
/// </summary>
public class SpeedJsonConverter : QuantityJsonConverter<Speed> {
	public override string GetSuffix() => "m/s";
    public override Speed ParseQuantity(double quant) => Speed.MetresPerSecond(quant);
    public override double GetQuantity(Speed value) => value.TotalMetresPerSecond;
}

/// <summary>
/// Measurement of the speed of astronomical objects
/// </summary>
[JsonConverter(typeof(SpeedJsonConverter))]
public class Speed : IArithmeticValue<Speed> {
    private double value;

    /// <summary>
    /// Total speed in m/s
    /// </summary>
    public double TotalMetresPerSecond => value;
    /// <summary>
    /// Total speed in km/hr
    /// </summary>
    public double TotalKilometresPerHour => TotalMetresPerSecond / (1000 /*m/km*/ * (1d / 3600d) /*s/hour*/);

    private Speed(double value) {
        this.value = value;
    }

    /// <summary>
    /// Static instance representing 0 speed
    /// </summary>
    public static readonly Speed Zero = new Speed(0);

    /// <summary>
    /// Create a speed in m/s
    /// </summary>
    /// <param name="speed">speed in m/s</param>
    /// <returns>speed</returns>
    public static Speed MetresPerSecond(double speed) {
        return new Speed(speed);
    }   

    /// <summary>
    /// Create a speed in km/hr
    /// </summary>
    /// <param name="speed">speed in km/hr</param>
    /// <returns>speed</returns>
    public static Speed KilometresPerHour(double speed) {
        return new Speed (speed * 1000 /*m/km*/ * (1d / 3600d) /*s/hour*/);
    }

    public Speed Add(Speed other) {
        return new Speed(this.value + other.value);
    }

    public static Speed operator + (Speed lhs, Speed rhs) {
		return new Speed(lhs.value + rhs.value);
	}

    public Speed Subtract(Speed other) {
        return new Speed(this.value - other.value);
    }

    public static Speed operator - (Speed lhs, Speed rhs) {
		return new Speed(lhs.value - rhs.value);
	}

    public Speed Multiply(Speed other) {
        return new Speed(this.value * other.value);
    }

    public Speed Divide(Speed other) {
        return new Speed(this.value / other.value);
    }

    public Speed Sqrt() {
        return new Speed(Math.Sqrt(this.value));
    }

    /// <summary>
	/// Scale this speed by a scalar value
	/// </summary>
	/// <param name="scale">scalar value</param>
	/// <returns>new speed</returns>
	public Speed Scale(double scale) {
		return new Speed(this.value * scale);
	}

    public override bool Equals(object obj) {
        if (obj is Speed real) {
            return this.value == real.value;
        } else 
            return base.Equals(obj);
    }

    public override int GetHashCode(){
        return value.GetHashCode();
    }
}

}