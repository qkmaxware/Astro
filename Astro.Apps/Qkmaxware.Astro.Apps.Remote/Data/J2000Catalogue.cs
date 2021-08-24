using System;
using System.Collections.Generic;
using System.Linq;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Astro.Query;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro.Apps.Remote.Data {

public class J2000Catalogue : NetworkCatalogue {

    KeplerianEntity earthJ2000 = new KeplerianEntity(
        name: "Earth",
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

    KeplerianEntity mercuryJ2000 = new KeplerianEntity(
        name: "Mercury",
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

    KeplerianEntity venusJ2000 = new KeplerianEntity(
        name: "Venus",
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

    KeplerianEntity marsJ2000 = new KeplerianEntity(
        name: "Mars",
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

    KeplerianEntity jupiterJ2000 = new KeplerianEntity(
        name: "Jupiter",
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

    KeplerianEntity saturnJ2000 = new KeplerianEntity(
        name: "Saturn",
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

    KeplerianEntity uranusJ2000 = new KeplerianEntity(
        name: "Uranus",
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

    KeplerianEntity neptuneJ2000 = new KeplerianEntity(
        name: "Neptune",
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
    
    private IEnumerable<KeplerianEntity> Planets() {
        yield return earthJ2000;
        yield return mercuryJ2000;
        yield return venusJ2000;
        yield return marsJ2000;
        yield return jupiterJ2000;
        yield return saturnJ2000;
        yield return uranusJ2000;
        yield return neptuneJ2000;
    }

    public override IEnumerable<CataloguedObject> Search(string name) {
        return Planets()
        .Where( planet => planet.Name.IndexOf(name, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
        .Select( planet => 
            new CataloguedObject {
                PlanetData = planet
            }
        );
    }
}

}