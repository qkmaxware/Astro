using System;
using Qkmaxware.Astro.Arithmetic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Qkmaxware.Astro {

public abstract class QuantityJsonConverter<T> : JsonConverter<T> where T:class{
	
    public abstract string GetSuffix();

    public abstract T ParseQuantity(double quant);
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