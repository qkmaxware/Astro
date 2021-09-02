using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Dynamics {

public class OrbitalPropagator {
    public Mass ParentMass {get; private set;}
    public OrbitalElements SatelliteOrbitalElements {get; private set;}

    public OrbitalPropagator(Mass parent, OrbitalElements initialOrbit) {
        this.ParentMass = parent;
        this.SatelliteOrbitalElements = initialOrbit;
    }

    public OrbitalPropagator Delay(Duration duration) {
        var mm = this.SatelliteOrbitalElements.MeanMotion(this.ParentMass);
        var n = mm.Amount * (1 / mm.Duration.TotalSiderealDays());
        var M = this.SatelliteOrbitalElements.MeanAnomaly + n * duration.TotalSiderealDays();

        return new OrbitalPropagator(
            this.ParentMass,
            new OrbitalElements(
                a:              this.SatelliteOrbitalElements.SemimajorAxis,
                i:              this.SatelliteOrbitalElements.Inclination,
                e:              this.SatelliteOrbitalElements.Eccentricity,
                Ω:              this.SatelliteOrbitalElements.LongitudeOfAscendingNode,
                w:              this.SatelliteOrbitalElements.ArgumentOfPeriapsis,
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