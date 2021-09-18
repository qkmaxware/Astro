using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Astro.Constants;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class OrbitalElementsTest {

    private OrbitalElements earthJ2000 = Constants.Planets.EarthJ2000.OrbitalElements;

    [TestMethod]
    public void Constructor() {
        // Test base defining properties
        Assert.AreEqual(1, (double)earthJ2000.SemimajorAxis.TotalAU(), 0.000001);
        Assert.AreEqual(0.00005, (double) earthJ2000.Inclination.TotalDegrees(), 0.000001);
        Assert.AreEqual(0.01671022, earthJ2000.Eccentricity);
        Assert.AreEqual(-11.26064, (double)earthJ2000.LongitudeOfAscendingNode.TotalDegrees(), 0.00001);
        Assert.AreEqual(102.94719, (double)earthJ2000.ArgumentOfPeriapsis.TotalDegrees(), 0.00001);

        // Test all anomalies
        Assert.AreEqual(100.46435, (double)earthJ2000.MeanAnomaly.TotalDegrees(), 0.00001);
        Assert.AreEqual(101.403, (double)earthJ2000.EccentricAnomaly.TotalDegrees(), 0.001);
        Assert.AreEqual(102.3399,(double)earthJ2000.TrueAnomaly.TotalDegrees(), 0.001);
    }

    private static void AssertApproximatelyEqual(Scientific expected, Scientific real, float tolerance = 0.0000001f) {
        if (expected.Exponent != real.Exponent)
            Assert.Fail($"Expected: <{expected}> - Actual <{real}>");
        if (Math.Abs(expected.Significand - real.Significand) > tolerance) {
            Assert.Fail($"Expected: <{expected}> - Actual <{real}>");
        }
    }

    [TestMethod]
    public void TestToStateVector() {
        var R = earthJ2000.CartesianPosition();
        Console.WriteLine("R: " + R);
        // Rx = -145616945390.06128m     = -1.4561694539006128e11
        var Rx = new Scientific(-1.4561694539006128, 11);
        AssertApproximatelyEqual(Rx, R.X.TotalMetres(), 0.000015f);
        // Ry = -36377792009.26875m      = -3.637779200926875e10
        var Ry = new Scientific(-3.637779200926875, 10);
        AssertApproximatelyEqual(Ry, R.Y.TotalMetres(), 0.000015f);
        // Rz = -55948.678183911616m     = -5.5948678183911616e4
        var Rz = new Scientific(-5.5948678183911616, 4);
        AssertApproximatelyEqual(Rz, R.Z.TotalMetres(), 0.000015f);
        
        var V = earthJ2000.CartesianVelocity(Constants.Planets.SolarMass);
        Console.WriteLine("V: " + V);
        // Vx = 6.722365304821024km/s           = 6.722365304821024e0km/s
        // Vy = -28.915358819013097km/s         = -2.8915358819013097e1km/s
        // Vz = -0.000023602102730285574km/s    = -2.3602102730285574e-5km/s
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
        Assert.AreEqual(149577185.30, (double)earthJ2000.SemiminorAxis().TotalKilometres(), 0.1);

        // 149598073 * (1 - 0.01671022) = 147098256.28859394
        Assert.AreEqual(147098256.28859394, 149598073 * (1 - 0.01671022)); // What it should be
        Assert.AreEqual(147098256.28859395, (double)earthJ2000.PeriapsisDistance().TotalKilometres(), 0.001);
        
        Assert.AreEqual(149556300.51279274, (double)earthJ2000.SemilatusRectum().TotalKilometres(), 0.001);
        Assert.AreEqual(365.0, (double)earthJ2000.OrbitalPeriod(Mass.Kilograms(1.989e30)).TotalDays(), 1);
    }

    [TestMethod]
    public void TestPropagation() {
        // Setup
        Moment reference = Moment.J2000;
        var referencePropagator = new OrbitalPropagator(Planets.SolarMass, earthJ2000);

        Moment now = new DateTime(2021, 09, 06, 12, 0, 0, DateTimeKind.Utc);
        var dt = now - reference;
        Assert.AreEqual(7919, (double)dt.TotalDays(), 0.1);
        var nowPropagator = referencePropagator.Delay(dt);
        var earthNow = nowPropagator.SatelliteOrbitalElements;

        // Asserts
        var period = earthJ2000.OrbitalPeriod(Planets.SolarMass);
        Assert.AreEqual(1, (double)period.TotalDays() / 365, 0.01); // Period of 1 year

        var mm = earthJ2000.MeanMotion(Planets.SolarMass);
        Assert.AreEqual((double)period.TotalDays(), (double)mm.Duration.TotalDays(), 0.000001); // Period of 1 year
        Assert.AreEqual(360, (double)mm.Amount.TotalDegrees(), 0.000001);                       // 360 degrees travelled

        Assert.AreEqual(100.46435, (double)earthJ2000.MeanAnomaly.TotalDegrees(), 0.00001);     // Confirm initial Mean Anomaly
        Console.WriteLine($"{mm.Amount.TotalDegrees()} / {mm.Duration.TotalSiderealDays()} = {mm.Amount.TotalDegrees() / mm.Duration.TotalSiderealDays()}");
        Console.WriteLine($"{mm.Amount.TotalDegrees()} / {mm.Duration.TotalSiderealDays()} * {dt.TotalSiderealDays()} = {(mm.Amount.TotalDegrees() / mm.Duration.TotalSiderealDays()) * dt.TotalSiderealDays()}");
        Console.WriteLine($"M(t) = {earthJ2000.MeanAnomaly} + ({mm.Amount} / {mm.Duration}) * {dt}");
        var nowMDegrees = earthJ2000.MeanAnomaly.TotalDegrees() + (mm.Amount.TotalDegrees() / mm.Duration.TotalSiderealDays()) * dt.TotalSiderealDays();
        var nowM = Angle.Degrees(nowMDegrees).Wrap();
        Console.WriteLine($"M(t) = {nowMDegrees}° = {nowM}");
        Assert.AreEqual(7910, (double)nowMDegrees, 10);      // Angle before wrapping
        Assert.AreEqual(346.3, (double)nowM.TotalDegrees(), 0.1);  // Angle after wrapping
        Assert.AreEqual((double)nowM.TotalDegrees(), (double)earthNow.MeanAnomaly.TotalDegrees(), 0.000001);
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