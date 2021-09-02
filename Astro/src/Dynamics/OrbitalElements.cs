using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

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
/// Keplerian orbital elements
/// </summary>
public class OrbitalElements {

    #region Orbital Elements

    /// <summary>
    /// The sum of the periapsis and apoapsis Lengths divided by two
    /// </summary>
    public Length SemimajorAxis {get; private set;}
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
    public Angle TrueAnomaly { get; private set; }
    /// <summary>
    /// The fraction of an elliptical orbit's period that has elapsed since the orbiting body passed periapsis
    /// </summary>
    public Angle MeanAnomaly { get; private set; }
    /// <summary>
    /// One of three angular parameters that define a position along an orbit
    /// </summary>
    public Angle EccentricAnomaly { get; private set; }

    #endregion

    #region Derived Quantitied
    // Note equations come from http://www.bogan.ca/orbits/kepler/orbteqtn.html 
    // Reference: "Fundamentals of Astrodynamics"

    /// <summary>
    /// Calculate the cartesian position of the orbiting object relative to the main body
    /// </summary>
    /// <param name="parent">body being orbited</param>
    /// <returns>position</returns>
    public Vec3<Length> CartesianPosition() {
        var nu = this.TrueAnomaly;
        var i = this.Inclination;
        var a = this.SemimajorAxis;
        var e = this.Eccentricity;
        var r = a * (1 - e * this.EccentricAnomaly.Cos());
        var O = this.LongitudeOfAscendingNode;
        var w = this.ArgumentOfPeriapsis;

        var x = r * (O.Cos() * (w + nu).Cos() - O.Sin() * (w + nu).Sin() * i.Cos());
        var y = r * (O.Sin() * (w + nu).Cos() + O.Cos() * (w + nu).Sin() * i.Cos());
        var z = r * (i.Sin() * (w + nu).Sin());

        return new Vec3<Length>(
            x,
            y,
            z
        );
    }

    /// <summary>
    /// Calculate the cartesian velocity of the orbiting object relative to the main body
    /// </summary>
    /// <param name="parent">body being orbited</param>
    /// <returns>velocity</returns>
    public Vec3<Speed> CartesianVelocity(Mass parent) {
        var nu = this.TrueAnomaly;
        var i = this.Inclination;
        var mu = parent.μ();
        var a = this.SemimajorAxis;
        var e = this.Eccentricity;
        var r = a * (1 - e * this.EccentricAnomaly.Cos());
        var h = (mu * a * (1 - Math.Pow(e, 2))).Sqrt();
        var O = this.LongitudeOfAscendingNode;
        var w = this.ArgumentOfPeriapsis;

        var x = r * (O.Cos() * (w + nu).Cos() - O.Sin() * (w + nu).Sin() * i.Cos());
        var y = r * (O.Sin() * (w + nu).Cos() + O.Cos() * (w + nu).Sin() * i.Cos());
        var z = r * (i.Sin() * (w + nu).Sin());

        var p = a * (1 - Math.Pow(e, 2));

        var dy = ((y.MultiplyBy(h) * e).DivideBy(r.MultiplyBy(p))) * nu.Sin() - (h.DivideBy(r)) * (O.Sin() * (w+nu).Sin() - O.Cos() * (w+nu).Cos() * i.Cos());
        var dz = ((z.MultiplyBy(h) * e).DivideBy(r.MultiplyBy(p))) * nu.Sin() + (h.DivideBy(r)) * ((w+nu).Cos() * i.Sin());
        var dx = ((x.MultiplyBy(h) * e).DivideBy(r.MultiplyBy(p))) * nu.Sin() - (h.DivideBy(r)) * (O.Cos() * (w+nu).Sin() + O.Sin() * (w+nu).Cos() * i.Cos());

        return new Vec3<Speed>(
            dx / Duration.OneSecond,
            dy / Duration.OneSecond,
            dz / Duration.OneSecond
        );
    }

    /// <summary>
    /// Specific orbital energy for this orbit
    /// </summary>
    /// <param name="parent">mass of body being orbitted</param>
    /// <returns>J/kg</returns>
    public Scientific SpecificOrbitalEnergy(Mass parent) {
        return -parent.μ() / (2 * SemimajorAxis.TotalMetres());
    }

    /// <summary>
    /// Total energy of a craft in this orbit
    /// </summary>
    /// <param name="parent">mass of body being orbitted</param>
    /// <param name="self">mass of body in orbit</param>
    /// <returns>orbital energy</returns>
    public Energy TotalEnergy(Mass parent, Mass self) {
        if (IsParabolic)
            return Energy.Joules(0);
        else {
            var specificOrbitalEnergy = SpecificOrbitalEnergy(parent);
            return Energy.Joules(self.TotalKilograms() /*kg*/ * specificOrbitalEnergy /*J/kg*/);
        }
    }

    /// <summary>
    /// Orbital period around a given mass
    /// </summary>
    /// <param name="parent">mass of body being orbitted</param>
    /// <returns>orbital period if the orbit is closes, otherwise infinity</returns>
    public Duration OrbitalPeriod(Mass parent) {
        if (IsParabolic || IsHyperbolic) {
            return Duration.Infinite;
        } else {
            var prefix = 4 * Math.PI * Math.PI;
            var a = this.SemimajorAxis.TotalMetres();
            var numerator = a * a * a;
            var denominator = parent.μ();
            return Duration.Seconds( ((prefix * numerator) / denominator).Sqrt() );
        }
    }
    /// <summary>
    /// Mean motion of an object in this orbit
    /// </summary>
    /// <param name="parent">mass of body being orbitted</param>
    /// <returns>mean motion</returns>
    public RateOfChange<Angle> MeanMotion(Mass parent) {
        return new RateOfChange<Angle>(
            Angle.Degrees(360),
            OrbitalPeriod(parent)
        );
    }
    private Duration timeAtAnomaly(Mass parent, Angle anomaly) {
        var a = this.SemimajorAxis.TotalMetres();
        var aaa = a * a * a;
        var u = parent.μ();
        var t = Math.Sqrt((double)(aaa / u)) * (anomaly.TotalRadians() - this.Eccentricity * Math.Sin((double)anomaly.TotalRadians()));
        return Duration.Seconds(t);
    }
    public Duration TimeSincePeriapsis(Mass parent) {
        var currentTime = timeAtAnomaly(parent, this.EccentricAnomaly);
        var timeAtPeriapsis = timeAtAnomaly(parent, Angle.Radians(0));
        return currentTime - timeAtPeriapsis;
    }
    public Duration TimeUntilPeriapsis(Mass parent) {
        return OrbitalPeriod(parent) - TimeSincePeriapsis(parent);
    }
    public Duration TimeSinceApoapsis(Mass parent) {
        var currentTime = timeAtAnomaly(parent, this.EccentricAnomaly);
        var timeAtApoapsis = timeAtAnomaly(parent, Angle.Radians(Math.PI));
        return currentTime - timeAtApoapsis;
    }
    public Duration TimeUntilApoapsis(Mass parent) {
        return OrbitalPeriod(parent) - TimeSinceApoapsis(parent);
    }
    /// <summary>
    /// The axis perpendicular to the Semimajor Axis
    /// </summary>
    public Length SemiminorAxis() => (SemimajorAxis * Math.Sqrt(1 - Eccentricity * Eccentricity));
    /// <summary>
    /// Length from the point that is closest to the body it orbits.
    /// </summary>
    public Length PeriapsisDistance() {
        if (IsParabolic) {
            return this.SemimajorAxis;
        } else {
            return (this.SemimajorAxis * (1 - this.Eccentricity));
        }
    }
    /// <summary>
    /// The length of the cord parallel to the conic section and running through a focus
    /// </summary>
    public Length SemilatusRectum() {
        if (IsParabolic) {
            return (2 * PeriapsisDistance());
        } 
        else { 
            return (this.SemimajorAxis * (1 - this.Eccentricity * this.Eccentricity)); 
        }
    }
    /// <summary>
    /// Is the orbit a circle
    /// </summary>
    public bool IsCircular => Eccentricity == 0;
    /// <summary>
    /// In the orbit an ellipse
    /// </summary>
    public bool IsEllipse => Eccentricity < 1;
    /// <summary>
    /// Is the orbit parabolic
    /// </summary>
    public bool IsParabolic => Eccentricity == 1;
    /// <summary>
    /// Is the orbit hyperbolic
    /// </summary>
    public bool IsHyperbolic => Eccentricity > 1;

    #endregion

    /// <summary>
    /// Create orbital elements from cartesian state vector
    /// </summary>
    /// <param name="parent">mass of body being orbited</param>
    /// <param name="positionVector">position relative to the parent object</param>
    /// <param name="velocityVector">velocity relative to the parent object</param>    
    public OrbitalElements(Mass parent, Vec3<Length> positionVector, Vec3<Speed> velocityVector) {
        var position = positionVector.Map(x => (Scientific)x.TotalMetres());
        var velocity = velocityVector.Map(x => (Scientific)x.TotalMetresPerSecond());
        var distance = position.Length;
        var speed = velocity.Length;
        var M = parent;

        // Cartesian state vector to orbital element conversion
        var H = Vec3<Scientific>.Cross(position, velocity);
        var h = H.Length;
        var up = new Vec3<Scientific>(0,0,1);
        var N = Vec3<Scientific>.Cross(up, H);
        var n = N.Length;

        var E = (Vec3<Scientific>.Cross(velocity,H) / M.μ()) - (position / distance);
        var e = (double)E.Length;

        var energy = (speed * speed)/2 - M.μ()/distance;

        Scientific a; Scientific p;
        if (Math.Abs(e - 1.0) > double.Epsilon) {
            a = -M.μ() / (2 * energy);
            p = a * (1 - e * e);
        } else {
            p = (h * h) / M.μ();
            a = double.PositiveInfinity;
        }

        double i = Math.Acos((double)(H.Z / h));

        double eps = double.Epsilon;
        double Omega; double w;
        if (Math.Abs(i) < eps) {
            Omega = 0; // For non-inclined orbits, this is undefined, set to 0 by convention
            if (Math.Abs(e) < eps) {
                w = 0; // For circular orbits, place periapsis at ascending node by convention
            }
            else {
                w = Math.Acos((double)(E.X / e)); 
            }
        } else {
            Omega = Math.Acos((double)(N.X / n));
            if (N.Y < 0) {
                Omega = (2 * Math.PI) - Omega;
            }

            w = Math.Acos((double)(Vec3<Scientific>.Dot(N, E) / (n * e)));
        }

        double nu;
        if (Math.Abs(e) < eps) {
            if (Math.Abs(i) < eps) {
                nu = Math.Acos((double)(position.X / distance));
                if (velocity.X > 0) {
                    nu = (2 * Math.PI) - nu;
                }
            } else {
                nu = Math.Acos((double)(Vec3<Scientific>.Dot(N,position) / (n * distance)));
                if (Vec3<Scientific>.Dot(N,velocity) > 0) {
                    nu = (2 * Math.PI) - nu;
                }
            }
        } else {
            if (E.Z < 0) {
                w = (2 * Math.PI) - w;
            }

            nu = Math.Acos((double)(Vec3<Scientific>.Dot(E, position) / (e * distance)));
            if (Vec3<Scientific>.Dot(position,velocity) < 0) {
                nu = (2 * Math.PI) - nu;
            }
        }

        this.SemimajorAxis = Length.Metres(a);
        this.Inclination = Angle.Radians(i);
        this.Eccentricity = e;
        this.LongitudeOfAscendingNode = Angle.Radians(Omega);
        this.ArgumentOfPeriapsis = Angle.Radians(w);
        this.AnomalyType = AnomalyType.True;
        var anomalyValue = Angle.Radians(nu);

        // Compute true anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.TrueAnomaly = Angle.Radians(mean2True(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.Eccentric:
                this.TrueAnomaly = Angle.Radians(eccentric2True(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.True:
            default:
                this.TrueAnomaly = anomalyValue.Wrap();
                break;
        }

        // Compute mean anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.MeanAnomaly = anomalyValue.Wrap();
                break;
            case AnomalyType.Eccentric:
                this.MeanAnomaly = Angle.Radians(eccentricToMean(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.True:
            default:
                this.MeanAnomaly = Angle.Radians(true2Mean(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
        }

        // Compute eccentric anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.EccentricAnomaly = Angle.Radians(mean2Eccentric(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.Eccentric:
                this.EccentricAnomaly = anomalyValue.Wrap();
                break;
            case AnomalyType.True:
            default:
                this.EccentricAnomaly = Angle.Radians(true2Eccentric(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
        }
    }

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
    public OrbitalElements (Length a, Angle i, double e, Angle @Ω, Angle @w, AnomalyType anomalyType, Angle anomalyValue) {
        this.SemimajorAxis = a;
        this.Inclination = i;
        this.Eccentricity = e;
        this.LongitudeOfAscendingNode = @Ω;
        this.ArgumentOfPeriapsis = @w;
        
        this.AnomalyType = anomalyType;

        // Compute true anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.TrueAnomaly = Angle.Radians(mean2True(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.Eccentric:
                this.TrueAnomaly = Angle.Radians(eccentric2True(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.True:
            default:
                this.TrueAnomaly = anomalyValue.Wrap();
                break;
        }

        // Compute mean anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.MeanAnomaly = anomalyValue.Wrap();
                break;
            case AnomalyType.Eccentric:
                this.MeanAnomaly = Angle.Radians(eccentricToMean(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.True:
            default:
                this.MeanAnomaly = Angle.Radians(true2Mean(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
        }

        // Compute eccentric anomaly
        switch (AnomalyType) {
            case AnomalyType.Mean:
                this.EccentricAnomaly = Angle.Radians(mean2Eccentric(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
                break;
            case AnomalyType.Eccentric:
                this.EccentricAnomaly = anomalyValue.Wrap();
                break;
            case AnomalyType.True:
            default:
                this.EccentricAnomaly = Angle.Radians(true2Eccentric(Eccentricity, (double)anomalyValue.Wrap().TotalRadians())).Wrap();
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