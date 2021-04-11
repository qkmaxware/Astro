using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class DurationTest {

    [TestMethod]
    public void TestMilliseconds() {
        var duration = Duration.Milliseconds(50);

        Assert.AreEqual(50, duration.TotalMilliseconds);
        Assert.AreEqual(0.05, duration.TotalSeconds);
        Assert.AreEqual(0.000833333, duration.TotalMinutes, 0.000001);
        Assert.AreEqual(1.3888e-5, duration.TotalHours, 0.0001);
    }

    [TestMethod]
    public void TestSeconds() {
        var duration = Duration.Seconds(45);

        Assert.AreEqual(45000, duration.TotalMilliseconds);
        Assert.AreEqual(45, duration.TotalSeconds);
        Assert.AreEqual(0.75, duration.TotalMinutes);
        Assert.AreEqual(0.0125, duration.TotalHours);
        Assert.AreEqual(0.00052083, duration.TotalDays, 0.000001);
    }

    [TestMethod]
    public void TestHours() {
        var duration = Duration.Hours(2.5);

        Assert.AreEqual(9000, duration.TotalSeconds);
        Assert.AreEqual(150, duration.TotalMinutes);
        Assert.AreEqual(2.5, duration.TotalHours);
        Assert.AreEqual(0.104167, duration.TotalDays, 0.0001);
    }

    [TestMethod]
    public void TestArithmetic() {
        var duration = Duration.Hours(2) + Duration.Minutes(25);
        Assert.AreEqual(120 + 25, duration.TotalMinutes);

        duration = Duration.Hours(5) - Duration.Minutes(256);
        Assert.AreEqual(44, duration.TotalMinutes);
    }

}

}