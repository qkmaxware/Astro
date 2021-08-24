using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Astro.Constants;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class OrbitalElementsTest {

    private OrbitalElements earthJ2000 = new OrbitalElements(
        a: AstronomicalLength.AU(1),
        i: Angle.Degrees(0.00005),
        e: 0.01671022,
        Ω: Angle.Degrees(-11.26064),
        w: Angle.Degrees(102.94719),
        AnomalyType.Mean,
        Angle.Degrees(100.46435)
    );

    [TestMethod]
    public void Constructor() {
        Assert.AreEqual(1, (double)earthJ2000.SemimajorAxis.TotalAU(), 0.000001);
        Assert.AreEqual(0.00005, (double) earthJ2000.Inclination.TotalDegrees(), 0.000001);
        Assert.AreEqual(0.01671022, earthJ2000.Eccentricity);
        Assert.AreEqual(101.403, (double)earthJ2000.EccentricAnomaly.TotalDegrees(), 0.001);
        Assert.AreEqual(102.34003,(double)earthJ2000.TrueAnomaly.TotalDegrees(), 0.001);
    }

    [TestMethod]
    public void TestDerivedProperties() {
        Assert.AreEqual(false, earthJ2000.IsCircular);
        Assert.AreEqual(true, earthJ2000.IsEllipse);
        Assert.AreEqual(false, earthJ2000.IsParabolic);
        Assert.AreEqual(false, earthJ2000.IsHyperbolic);

        // 1AU = 149598073Km
        Assert.AreEqual(149598073,(double)earthJ2000.SemimajorAxis.TotalKilometres());
        Assert.AreEqual(0.01671022, earthJ2000.Eccentricity);
        Assert.AreEqual(0.98328978, 1 - earthJ2000.Eccentricity);
        
        // 149598073 * Math.Sqrt(1 - 0.01671022*0.01671022) = 149577185.30
        Assert.AreEqual(149577185.30, 149598073 * Math.Sqrt(1 - 0.01671022*0.01671022), 0.1); // What it should be
        Assert.AreEqual(149577185.30, (double)earthJ2000.SemiminorAxis().TotalKilometres(), 0.001);

        // 149598073 * (1 - 0.01671022) = 147098256.28859394
        Assert.AreEqual(147098256.28859394, 149598073 * (1 - 0.01671022)); // What it should be
        Assert.AreEqual(147098256.28859395, (double)earthJ2000.PeriapsisDistance().TotalKilometres(), 0.001);
        
        Assert.AreEqual(149556300.51279274, (double)earthJ2000.SemilatusRectum().TotalKilometres(), 0.001);
        Assert.AreEqual(365.0, (double)earthJ2000.OrbitalPeriod(Mass.Kilograms(1.989e30)).TotalDays(), 1);
    }

    //[TestMethod]
    public void TestPropagation() {
        // Setup
        Moment reference = Moment.J2000;
        var referencePropagator = new OrbitalPropagator(Masses.Sun, Masses.Earth, earthJ2000);

        Moment now = new DateTime(2021, (int)Month.Aug, 21, 12, 0, 0);
        var nowPropagator = referencePropagator.Delay(now - reference);
        var earthNow = nowPropagator.SatelliteOrbitalElements;

        // Asserts
        
    }

    //[TestMethod]
    public void TestManeuvers() {
        // Hohmann transfer
        // https://www.instructables.com/Calculating-a-Hohmann-Transfer/
        /*
        var sunMass = Mass.Kilograms(1.989e30);
        var earthMass = Mass.Kilograms(5.972e24);
        var earthJ2000 = new OrbitalElements(
            a: Distance.AU(1),
            i: Angle.Degrees(0.00005),
            e: 0.01671022,
            Ω: Angle.Degrees(-11.26064),
            w: Angle.Degrees(102.94719),
            AnomalyType.Mean,
            Angle.Degrees(100.46435)
        );
        var marsMass = Mass.Kilograms(6.39e23);
        var marsJ2000 = new OrbitalElements(
            a: Distance.AU(1.5237),
            i: Angle.Degrees(1.85061),
            e: 0.09341233,
            Ω: Angle.Degrees(49.57854),
            w: Angle.Degrees(336.04084),
            AnomalyType.Mean,
            Angle.Degrees(355.45332)
        );
        var hohmannOrbitSemiMajorAxis = (earthJ2000.SemimajorAxis + marsJ2000.SemimajorAxis) / 2;
        var hohmannTransferPeriod = Math.Sqrt(4 * Math.PI * Math.PI * hohmannOrbitSemiMajorAxis * hohmannOrbitSemiMajorAxis * hohmannOrbitSemiMajorAxis / sunMass.μ);
        var v1 = (2 * Math.PI * earthJ2000.SemimajorAxis.TotalMetres) / earthJ2000.OrbitalPeriod(sunMass).TotalSeconds;
        var v2 = (2 * Math.PI * marsJ2000.SemimajorAxis.TotalMetres) / marsJ2000.OrbitalPeriod(sunMass).TotalSeconds;
        var vp = (2 * Math.PI * hohmannOrbitSemiMajorAxis.TotalMetres / hohmannTransferPeriod.TotalSeconds) * Math.Sqrt((2 * hohmannOrbitSemiMajorAxis.TotalMetres / earthJ2000.SemimajorAxis.TotalMetres) - 1);
        
        var dv1 = vp - v1;

        var va = (2 * Math.PI * hohmannOrbitSemiMajorAxis.TotalMetres / hohmannTransferPeriod.TotalSeconds) * Math.Sqrt((2 * hohmannOrbitSemiMajorAxis.TotalMetres / marsJ2000.SemimajorAxis.TotalMetres) - 1);

        var dv2 = v2 - va;

        */
    }
}

}