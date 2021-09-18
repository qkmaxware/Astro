using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Dynamics {

public class OrbitalPropagator {
    public Mass ParentMass {get; private set;}
    public OrbitalElements SatelliteOrbitalElements {get; private set;}
    public OrbitalElementRates? RatesOfChange {get; private set;}

    public OrbitalPropagator(Mass parent, OrbitalElements initialOrbit) : this(parent, initialOrbit, null) {}
    public OrbitalPropagator(Mass parent, OrbitalElements initialOrbit, OrbitalElementRates? ratesOfChange) {
        this.ParentMass = parent;
        this.SatelliteOrbitalElements = initialOrbit;
        this.RatesOfChange = ratesOfChange;
    }

    private T ChangeAmountOverDuration<T>(RateOfChange<T>? rate, T @default, Duration duration) where T:IScalable<Scientific,T> {
        if (rate == null) {
            return @default;
        } else {
            var scalar = (duration.TotalSiderealDays() / rate.Duration.TotalSiderealDays());
            return rate.Amount.ScaleBy(scalar);
        }
    }

    private RateOfChange<Scientific>? ToScientific(RateOfChange<double>? d) {
        if (d != null) {
            return new RateOfChange<Scientific>(new Scientific(d.Amount), d.Duration);
        } else {
            return null;
        }
    }

    public OrbitalPropagator Delay(Duration duration) {
        // Propagate orbital elements forward in time if rates of change exist
        var da = ChangeAmountOverDuration(this.RatesOfChange?.SemimajorAxis, Length.Zero, duration);
        var di = ChangeAmountOverDuration(this.RatesOfChange?.Inclination, Angle.Zero, duration);
        var de = (double)ChangeAmountOverDuration(ToScientific(this.RatesOfChange?.Eccentricity), Scientific.Zero, duration);
        var dO = ChangeAmountOverDuration(this.RatesOfChange?.LongitudeOfAscendingNode, Angle.Zero, duration);
        var dw = ChangeAmountOverDuration(this.RatesOfChange?.ArgumentOfPeriapsis, Angle.Zero, duration);

        var a = this.SatelliteOrbitalElements.SemimajorAxis + da;
        var i = this.SatelliteOrbitalElements.Inclination + di;
        var e = this.SatelliteOrbitalElements.Eccentricity + de;
        var O = this.SatelliteOrbitalElements.LongitudeOfAscendingNode + dO;
        var w = this.SatelliteOrbitalElements.ArgumentOfPeriapsis + dw; 

        // Compute the new mean anomaly
        var mm = this.SatelliteOrbitalElements.MeanMotion(this.ParentMass);
        var n = mm.Amount * (1 / mm.Duration.TotalSiderealDays());
        var M = this.SatelliteOrbitalElements.MeanAnomaly + n * duration.TotalSiderealDays();

        return new OrbitalPropagator(
            this.ParentMass,
            new OrbitalElements(
                a:              a,
                i:              i,
                e:              e,
                Ω:              O,
                w:              w,
                anomalyType:    AnomalyType.Mean,
                anomalyValue:   M
            )
        );
    }

    public OrbitalPropagator AddProgradeDeltaV(Speed s) {
        var v = this.SatelliteOrbitalElements.CartesianVelocity(this.ParentMass).Normalized;
        var dv = s * v;
        return AddDeltaV(dv);
    }
    
    public OrbitalPropagator AddRetrogradeDeltaV(Speed s) {
        var v = this.SatelliteOrbitalElements.CartesianVelocity(this.ParentMass).Normalized;
        var dv = s * v;
        return AddDeltaV(-dv);
    }

    public OrbitalPropagator AddDeltaV(Vec3<Speed> dv) {
        var p = this.SatelliteOrbitalElements.CartesianPosition();
        var v = this.SatelliteOrbitalElements.CartesianVelocity(this.ParentMass);
        
        return new OrbitalPropagator(
            this.ParentMass,
            new OrbitalElements(this.ParentMass, p, v + dv)
        );
    }
}

}