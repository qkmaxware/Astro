using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Dynamics {

public class OrbitalPropagator {
    public Mass ParentMass {get; private set;}
    public Mass SatelliteMass {get; private set;}
    public OrbitalElements SatelliteOrbitalElements {get; private set;}

    public OrbitalPropagator(Mass parent, Mass satellite, OrbitalElements initialOrbit) {
        this.ParentMass = parent;
        this.SatelliteMass = satellite;
        this.SatelliteOrbitalElements = initialOrbit;
    }

    public OrbitalPropagator Delay(Duration duration) {
        var mm = this.SatelliteOrbitalElements.MeanMotion(this.ParentMass);
        var rate = mm.Amount.TotalRadians() / mm.Duration.TotalSeconds();
        var position = this.SatelliteOrbitalElements.MeanAnomaly.TotalRadians();
        var new_position = (double)(position + rate * duration.TotalSeconds()) % (2 * Math.PI);
        return new OrbitalPropagator(
            this.ParentMass,
            this.SatelliteMass,
            new OrbitalElements(
                a:              this.SatelliteOrbitalElements.SemimajorAxis,
                i:              this.SatelliteOrbitalElements.Inclination,
                e:              this.SatelliteOrbitalElements.Eccentricity,
                Ω:              this.SatelliteOrbitalElements.LongitudeOfAscendingNode,
                w:              this.SatelliteOrbitalElements.ArgumentOfPeriapsis,
                anomalyType:    AnomalyType.Mean,
                anomalyValue:   Angle.Radians(new_position)
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
        var p = this.SatelliteOrbitalElements.CartesianPosition(this.ParentMass);
        var v = this.SatelliteOrbitalElements.CartesianVelocity(this.ParentMass);
        
        return new OrbitalPropagator(
            this.ParentMass,
            this.SatelliteMass,
            new OrbitalElements(this.ParentMass, p, v + dv)
        );
    }
}

}