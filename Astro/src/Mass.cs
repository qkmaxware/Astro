using System.Text.Json.Serialization;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {

/// <summary>
/// Json converter for mass quantities
/// </summary>
public class MassJsonConverter : QuantityJsonConverter<Mass> {
	public override string GetSuffix() => "kg";
    public override Mass ParseQuantity(double quant) => Mass.Kilograms(quant);
    public override double GetQuantity(Mass value) => value.TotalKilograms;
}

/// <summary>
/// Measurement of mass
/// </summary>
[JsonConverter(typeof(MassJsonConverter))]
public class Mass : IArithmeticValue<Mass> {
    private double value;
    private static readonly double G = 6.674e-11;

    /// <summary>
    /// Total mass measured in kilograms
    /// </summary>
    public double TotalKilograms => value;

    /// <summary>
    /// Total mass measured in grams
    /// </summary>
    public double TotalGrams => value * 1000;

    /// <summary>
    /// Standard gravitational parametre for this mass
    /// </summary>
    /// <returns>standard gravitational parametre</returns>
    public double μ => G * this.TotalKilograms;

    private Mass(double value) {
        this.value = value;
    }

    /// <summary>
    /// Static instance representing 0 mass
    /// </summary>
    public static readonly Mass Zero = new Mass(0);
    /// <summary>
    /// Static instance representing the same mass as the Earth
    /// </summary>
    public static readonly Mass Earth = Mass.Kilograms(5.972e24);

    /// <summary>
    /// Create a mass measured in grams
    /// </summary>
    /// <param name="g">mass in grams</param>
    /// <returns>mass</returns>
    public static Mass Grams(double g) {
        return Kilograms(g / 1000d);
    }

    /// <summary>
    /// Create a mass in kg
    /// </summary>
    /// <param name="kg">mass in kg</param>
    /// <returns>mass</returns>
    public static Mass Kilograms(double kg) {
        return new Mass(kg);
    }

    public Mass Add(Mass other) {
        return new Mass(this.value + other.value);
    }

    public static Mass operator + (Mass lhs, Mass rhs) {
		return new Mass(lhs.value + rhs.value);
	}

    public Mass Subtract(Mass other) {
        return new Mass(this.value - other.value);
    }

    public static Mass operator - (Mass lhs, Mass rhs) {
		return new Mass(lhs.value - rhs.value);
	}

    public Mass Multiply(Mass other) {
        return new Mass(this.value * other.value);
    }

    public Mass Divide(Mass other) {
        return new Mass(this.value / other.value);
    }

    public Mass Sqrt() {
        return new Mass(System.Math.Sqrt(this.value));
    }

    /// <summary>
	/// Scale this mass by a scalar value
	/// </summary>
	/// <param name="scale">scalar value</param>
	/// <returns>new mass</returns>
	public Mass Scale(double scale) {
		return new Mass(this.value * scale);
	}
}

}