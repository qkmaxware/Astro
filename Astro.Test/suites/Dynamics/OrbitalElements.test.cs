using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Astro.Dynamics;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class OrbitalElementsTest {
    [TestMethod]
    public void Constructor() {
        var earthJ2000 = new OrbitalElements(
            a: Distance.AU(1),
            i: Angle.Degrees(0.00005),
            e: 0.01671022,
            Ω: Angle.Degrees(-11.26064),
            ω: Angle.Degrees(102.94719),
            AnomalyType.Mean,
            Angle.Degrees(100.46435)
        );

        //earthJ2000.EccentricAnomaly;
        //earthJ2000.TrueAnomaly;
    }
}

}