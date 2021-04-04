using System;
using Qkmaxware.Astro.Arithmetic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Qkmaxware.Astro {

/// <summary>
/// Base class for converting custom quantities to json
/// </summary>
/// <typeparam name="T">type of quantity to convert</typeparam>
public abstract class QuantityJsonConverter<T> : JsonConverter<T> where T:class{
	/// <summary>
	/// Suffix used for quantities, typically a unit of measure
	/// </summary>
	/// <returns>suffix</returns>
    public abstract string GetSuffix();

	/// <summary>
	/// Parse a double to a quantity object
	/// </summary>
	/// <param name="quant">double</param>
	/// <returns>object with desired quantity</returns>
    public abstract T ParseQuantity(double quant);
	/// <summary>
	/// Convert a quantity to a double
	/// </summary>
	/// <param name="value">quantity</param>
	/// <returns>numeric value in the given unit of measure</returns>
    public abstract double GetQuantity(T value);

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		// Read string, if null return zero for the quantity
		var str = reader.GetString();
		if (str == null || str.Length < 1) {
			return default(T);
		}

		// Strip the suffix
        var suffix = GetSuffix();
		if (str.EndsWith(suffix)) {
			str = str.Remove(str.Length - suffix.Length);
		}

		// Return the quantity
		return ParseQuantity(double.Parse(str));
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		// Add suffix to quantity value
        var suffix = GetSuffix();
		writer.WriteStringValue(GetQuantity(value) + suffix);
	}
}

}