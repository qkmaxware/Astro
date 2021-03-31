using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class AngleTest {

    [TestMethod]
    public void TestDegrees () {
        var angle = Angle.Degrees(45);
        Assert.AreEqual(45, angle.TotalDegrees, 0.001);
        Assert.AreEqual(0.785398, angle.TotalRadians, 0.001);
        Assert.AreEqual(3, angle.TotalHours, 0.001);
    }

    [TestMethod]
    public void TestRadians () {
        var angle = Angle.Radians(0.785398);
        Assert.AreEqual(45, angle.TotalDegrees, 0.001);
        Assert.AreEqual(0.785398, angle.TotalRadians, 0.001);
        Assert.AreEqual(3, angle.TotalHours, 0.001);
    }

    [TestMethod]
    public void TestHours () {
        var angle = Angle.Hours(3);
        Assert.AreEqual(45, angle.TotalDegrees, 0.001);
        Assert.AreEqual(0.785398, angle.TotalRadians, 0.001);
        Assert.AreEqual(3, angle.TotalHours, 0.001);
    }

}

}