using System.Collections.Generic;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Astro.Query;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Constants {

/// <summary>
/// Keplerian entity with additional metadata for planetary information
/// </summary>
public class PlanetaryEntity : KeplerianEntity {
    /// <summary>
    /// Mass of the planet
    /// </summary>
    /// <value>mass</value>
    public Mass Mass {get; private set;}

    /// <summary>
    /// Approximate planet radius
    /// </summary>
    /// <value>Approximate radius</value>
    public Length Radius {get; private set;}

    /// <summary>
    /// Approximate planetary diametre
    /// </summary>
    /// <returns>Approximate diametre</returns>
    public Length Diametre() => 2 * Radius;

    /// <summary>
    /// Approximate apparent radius as viewed from the given distance
    /// </summary>
    /// <param name="atDistance">observation distance</param>
    /// <returns>Approximate apparent size</returns>
    public Angle ApparentRadius(Length atDistance) => 2 * Angle.Atan2((double)Radius.TotalKilometres(), (double)atDistance.TotalKilometres());

    public PlanetaryEntity(string name, Mass mass, Length radius, Moment epoch, OrbitalElements elements) : base(name, epoch, elements) {
        this.Mass = mass;
        this.Radius = radius;
    }
}

public static class Planets {
    /// <summary>
    /// Mass of the sun
    /// </summary>
    /// <returns>mass</returns>
    public static readonly Mass SolarMass = Mass.Kilograms(new Scientific(1.989, 30));

    public static readonly PlanetaryEntity EarthJ2000 = new PlanetaryEntity(
        name: "Earth",
        mass: Mass.Kilograms(new Scientific(5.972, 24)),
        radius: Length.Kilometres(6371),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a: AstronomicalLength.AU(1),
            i: Angle.Degrees(0.00005),
            e: 0.01671022,
            Ω: Angle.Degrees(-11.26064),
            w: Angle.Degrees(102.94719),
            AnomalyType.Mean,
            Angle.Degrees(100.46435)
        )
    );

    public static readonly PlanetaryEntity MercuryJ2000 = new PlanetaryEntity(
        name: "Mercury",
        mass: Mass.Kilograms(new Scientific(3.285, 23)),
        radius: Length.Kilometres(2439.7),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(0.38709893),
            i:  Angle.Degrees(7.00487), 
            e:  0.20563069, 
            @Ω: Angle.Degrees(48.33167), 
            w: Angle.Degrees(77.45645), 
            AnomalyType.Mean,
            Angle.Degrees(252.25084)
        )
    );

    public static readonly PlanetaryEntity VenusJ2000 = new PlanetaryEntity(
        name: "Venus",
        mass: Mass.Kilograms(new Scientific(4.867, 24)),
        radius: Length.Kilometres(6051.8),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(0.72333199),
            i:  Angle.Degrees(3.39471), 
            e:  0.00677323, 
            @Ω: Angle.Degrees(76.68069), 
            w: Angle.Degrees(131.53298), 
            AnomalyType.Mean,
            Angle.Degrees(181.97973)
        )
    );

    public static readonly PlanetaryEntity MarsJ2000 = new PlanetaryEntity(
        name: "Mars",
        mass: Mass.Kilograms(new Scientific(6.39, 23)),
        radius: Length.Kilometres(3389.5),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(1.52366231),
            i:  Angle.Degrees(1.85061), 
            e:  0.09341233, 
            @Ω: Angle.Degrees(49.57854), 
            w: Angle.Degrees(336.04084), 
            AnomalyType.Mean,
            Angle.Degrees(355.45332)
        )
    );

    public static readonly PlanetaryEntity JupiterJ2000 = new PlanetaryEntity(
        name: "Jupiter",
        mass: Mass.Kilograms(new Scientific(1.898, 27)),
        radius: Length.Kilometres(69911),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(5.20336301),
            i:  Angle.Degrees(1.30530), 
            e:  0.04839266, 
            @Ω: Angle.Degrees(100.55615), 
            w: Angle.Degrees(14.75385), 
            AnomalyType.Mean,
            Angle.Degrees(34.40438)
        )
    );

    public static readonly PlanetaryEntity SaturnJ2000 = new PlanetaryEntity(
        name: "Saturn",
        mass: Mass.Kilograms(new Scientific(5.683, 26)),
        radius: Length.Kilometres(58232),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(9.53707032),
            i:  Angle.Degrees(2.48446), 
            e:  0.05415060, 
            @Ω: Angle.Degrees(113.71504), 
            w: Angle.Degrees(92.43194), 
            AnomalyType.Mean,
            Angle.Degrees(49.94432)
        )
    );

    public static readonly PlanetaryEntity UranusJ2000 = new PlanetaryEntity(
        name: "Uranus",
        mass: Mass.Kilograms(new Scientific(8.681, 25)),
        radius: Length.Kilometres(25362),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(19.19126393),
            i:  Angle.Degrees(0.76986), 
            e:  0.04716771, 
            @Ω: Angle.Degrees(74.22988), 
            w: Angle.Degrees(170.96424), 
            AnomalyType.Mean,
            Angle.Degrees(313.23218)
        )
    );

    public static readonly PlanetaryEntity NeptuneJ2000 = new PlanetaryEntity(
        name: "Neptune",
        mass: Mass.Kilograms(new Scientific(1.024, 26)),
        radius: Length.Kilometres(24622),
        epoch: Moment.J2000,
        elements: new OrbitalElements(
            a:  AstronomicalLength.AU(30.06896348),
            i:  Angle.Degrees(1.76917), 
            e:  0.00858587, 
            @Ω: Angle.Degrees(131.72169), 
            w: Angle.Degrees(44.97135), 
            AnomalyType.Mean,
            Angle.Degrees(304.88003)
        )
    );
    
    public static IEnumerable<PlanetaryEntity> AtJ2000() {
        yield return EarthJ2000;
        yield return MercuryJ2000;
        yield return VenusJ2000;
        yield return MarsJ2000;
        yield return JupiterJ2000;
        yield return SaturnJ2000;
        yield return UranusJ2000;
        yield return NeptuneJ2000;
    }
}

}