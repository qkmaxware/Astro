using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qkmaxware.Astro.Dynamics {

/// <summary>
/// Types of orbit anomalies
/// </summary>
public enum AnomalyType {
    True, 
    Mean, 
    Eccentric
}

/// <summary>
/// Class to convert orbital elements to and from json
/// </summary>
public class OrbitalElementJsonConverter : JsonConverter<OrbitalElements> {
    public override OrbitalElements? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null) {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
		    throw new JsonException("Expected StartObject token");

        Distance? a = null;
        Angle? i = null;
        double e = 0;
        Angle? O = null;
        Angle? w = null;
        Angle? v = null;

        while (reader.Read()) {
            switch (reader.TokenType) {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndArray:
                    break;
                case JsonTokenType.Number:
                case JsonTokenType.String:
                case JsonTokenType.PropertyName:
                    var name = reader.GetString();
                    switch (name) {
                        case "a":
                            a = JsonSerializer.Deserialize<Distance>(ref reader, options);
                            break;
                        case "i":
                            i = JsonSerializer.Deserialize<Angle>(ref reader, options);
                            break;
                        case "e":
                            e = JsonSerializer.Deserialize<double>(ref reader, options);
                            break;
                        case "Ω":
                            O = JsonSerializer.Deserialize<Angle>(ref reader, options);
                            break;
                        case "w":
                            w = JsonSerializer.Deserialize<Angle>(ref reader, options);
                            break;
                        case "v":
                            v = JsonSerializer.Deserialize<Angle>(ref reader, options);
                            break;
                    }
                    break;
                case JsonTokenType.EndObject:
                    if (a == null || i == null || O == null || w == null || v == null) {
                        throw new JsonException("Object is missing required orbital element properties");
                    } else {
                        return new OrbitalElements(a, i, e, O, w, AnomalyType.True, v);
                    }
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, OrbitalElements value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WritePropertyName("a");
        JsonSerializer.Serialize(writer, value.SemimajorAxis, options);

        writer.WritePropertyName("i");
        JsonSerializer.Serialize(writer, value.Inclination, options);

        writer.WritePropertyName("e");
        JsonSerializer.Serialize(writer, value.Eccentricity, options);

        writer.WritePropertyName("Ω");
        JsonSerializer.Serialize(writer, value.LongitudeOfAscendingNode, options);

        writer.WritePropertyName("w");
        JsonSerializer.Serialize(writer, value.ArgumentOfPeriapsis, options);
        
        writer.WritePropertyName("v");
        JsonSerializer.Serialize(writer, value.TrueAnomaly, options);
        writer.WriteEndObject();
    }
}

/// <summary>
/// Keplerian orbital elements
/// </summary>
[JsonConverter(typeof(OrbitalElementJsonConverter))]
public class OrbitalElements {

    #region Orbital Elements

    /// <summary>
    /// The sum of the periapsis and apoapsis distances divided by two
    /// </summary>
    public Distance SemimajorAxis {get; private set;}
    /// <summary>
    /// Vertical tilt of the ellipse with respect to the reference plane
    /// </summary>
    public Angle Inclination {get; private set;}
    /// <summary>
    /// Shape of the ellipse, describing how much it is elongated compared to a circle
    /// </summary>
    public double Eccentricity {get; private set;}
    /// <summary>
    /// Horizontally orients the ascending node of the ellipse
    /// </summary>
    public Angle LongitudeOfAscendingNode {get; private set;}
    /// <summary>
    /// The angle between the direction of periapsis and the current position of the body
    /// </summary>
    public Angle ArgumentOfPeriapsis {get; private set;}
    /// <summary>
    /// Type of anomaly used to create this orbit
    /// </summary>
    public AnomalyType AnomalyType {get; private set;}
    /// <summary>
    /// The angle between the direction of periapsis and the current position of the body
    /// </summary>
    public Angle TrueAnomaly {get; private set;}
    /// <summary>
    /// The fraction of an elliptical orbit's period that has elapsed since the orbiting body passed periapsis
    /// </summary>
    public Angle MeanAnomaly {get; private set;}
    /// <summary>
    /// One of three angular parameters that define a position along an orbit
    /// </summary>
    public Angle EccentricAnomaly {get; private set;}

    #endregion

    #region Derived Quantitied

    /// <summary>
    /// The axis perpendicular to the Semimajor Axis
    /// </summary>
    public Distance SemiminorAxis => (SemimajorAxis * (1 - Eccentricity));
    /// <summary>
    /// Distance from the point that is closest to the body it orbits.
    /// </summary>
    public Distance PeriapsisDistance {
        get {
            if (IsParabolic) {
                return this.SemimajorAxis;
            } else {
                return (this.SemimajorAxis * (1 - this.Eccentricity));
            }
        }
    }
    /// <summary>
    /// The length of the cord parallel to the conic section and running through a focus
    /// </summary>
    public Distance SemilatusRectum {
        get {
            if (IsParabolic) {
                return (2 * PeriapsisDistance);
            } 
            else { 
                return (this.SemimajorAxis * (1 - this.Eccentricity * this.Eccentricity)); 
            }
        }
    }
    /// <summary>
    /// Is the orbit a circle
    /// </summary>
    private bool IsCircular => Eccentricity == 0;
    /// <summary>
    /// In the orbit an ellipse
    /// </summary>
    private bool IsEllipse => Eccentricity < 1;
    /// <summary>
    /// Is the orbit parabolic
    /// </summary>
    private bool IsParabolic => Eccentricity == 1;
    /// <summary>
    /// Is the orbit hyperbolic
    /// </summary>
    private bool IsHyperbolic => Eccentricity > 1;

    #endregion

    /// <summary>
    /// Create a new orbital element collection
    /// </summary>
    /// <param name="a">semimajor-axis</param>
    /// <param name="i">angle of inclination</param>
    /// <param name="e">eccentricity</param>
    /// <param name="Ω">longitude of ascending node</param>
    /// <param name="ω">argument of periapsis</param>
    /// <param name="anomalyValue">anomaly value</param>
    /// <param name="anomalyType">type of anomaly</param>
    public OrbitalElements (Distance a, Angle i, double e, Angle @Ω, Angle @ω, AnomalyType anomalyType, Angle anomalyValue) {
        this.SemimajorAxis = a;
        this.Inclination = i;
        this.Eccentricity = e;
        this.LongitudeOfAscendingNode = @Ω;
        this.ArgumentOfPeriapsis = @ω;

        this.AnomalyType = anomalyType;

        // Compute true anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.TrueAnomaly = Angle.Radians(mean2True(Eccentricity, anomalyValue.TotalRadians));
                break;
            case AnomalyType.Eccentric:
                this.TrueAnomaly = Angle.Radians(eccentric2True(Eccentricity, anomalyValue.TotalRadians));
                break;
            case AnomalyType.True:
            default:
                this.TrueAnomaly = anomalyValue;
                break;
        }

        // Compute mean anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.MeanAnomaly = anomalyValue;
                break;
            case AnomalyType.Eccentric:
                this.MeanAnomaly = Angle.Radians(eccentricToMean(Eccentricity, anomalyValue.TotalRadians));
                break;
            case AnomalyType.True:
            default:
                this.MeanAnomaly = Angle.Radians(true2Mean(Eccentricity, anomalyValue.TotalRadians));
                break;
        }

        // Compute eccentric anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.EccentricAnomaly = Angle.Radians(mean2Eccentric(Eccentricity, anomalyValue.TotalRadians));
                break;
            case AnomalyType.Eccentric:
                this.EccentricAnomaly = anomalyValue;
                break;
            case AnomalyType.True:
            default:
                this.EccentricAnomaly = Angle.Radians(true2Eccentric(Eccentricity, anomalyValue.TotalRadians));
                break;
        }
    }


    # region Anomaly Conversion Utilities

    /// <summary>
    /// Convert mean anomaly to true anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="M">mean anomaly radians</param>
    /// <returns>true anomaly radians</returns>
    private static double mean2True(double e, double M) {
        // https://en.wikipedia.org/wiki/Mean_anomaly
        // This is a fourth order approximation. It is by no means precise
        var first = M;
        var second = (2d * e - (1d/4d) * (e * e * e)) * Math.Sin(M);
        var third = (5d/4d) * (e * e) * Math.Sin(2d * M);
        var fourth = (13d/12d) * (e * e * e) * Math.Sin(3d * M);
        
        return first + second + third + fourth;
    }

    /// <summary>
    /// Convert mean anomaly to true anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="M">mean anomaly radians</param>
    /// <returns>eccentric anomaly radians</returns>
    private static double mean2Eccentric (double e, double M, int decimalPlaces = 8) {
        // Find roots of f(E) = E - e*sin(E) - M
        var iterations = 30; 
        var i = 0;
        var delta = Math.Pow(10, -decimalPlaces);

        // Newton's method        
        double En  = Math.PI; // initial guess

        while (i < iterations) {
            double En1 = En - (En - e * Math.Sin(En) - M) / (1 - e * Math.Cos(En));
            if (Math.Abs(En1 - En) < delta) {
                En = En1;
                break;
            } else {
                En = En1;
                continue;
            }
        }

        return En;
    }

    /// <summary>
    /// Convert true anomaly to mean anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="T">true anomaly radians</param>
    /// <returns>mean anomaly radians</returns>
    private static double true2Mean(double e, double T) {
        var E = true2Eccentric(e, T);
        return eccentricToMean(e, E);
    }

    /// <summary>
    /// Convert true anomaly to eccentric anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="T">true anomaly radians</param>
    /// <returns>eccentric anomaly radians</returns>
    private static double true2Eccentric(double e, double T) {
        return Math.Atan2(
            Math.Sqrt(1 - e * e) * Math.Sin(T),
            e + Math.Cos(T)
        );
    }

    /// <summary>
    /// Convert eccentric anomaly to mean anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="E">eccentric anomaly radians</param>
    /// <returns>mean anomaly radians</returns>
    private static double eccentricToMean(double e, double E) {
        return E - e * Math.Sin(E);
    }

    /// <summary>
    /// Convert eccentric anomaly to true anomaly
    /// </summary>
    /// <param name="e">eccentricity</param>
    /// <param name="E">eccentric anomaly radians</param>
    /// <returns>true anomaly radians</returns>
    private static double eccentric2True(double e, double E) {
        // https://en.wikipedia.org/wiki/Eccentric_anomaly
        var sqrt = Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E / 2);
        return 2 * Math.Atan(sqrt);
    }

    #endregion
}

}