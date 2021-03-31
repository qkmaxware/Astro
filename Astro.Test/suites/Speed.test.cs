using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class SpeedTest {
    
    [TestMethod]
    public void TestMetresPerSecond() {
        var speed = Speed.MetresPerSecond(1000);
        Assert.AreEqual(1000, speed.TotalMetresPerSecond);
        Assert.AreEqual(3600, speed.TotalKilometresPerHour);
    }

    [TestMethod]
    public void TestKilometresPerHour() {
        var speed = Speed.KilometresPerHour(3600);
        Assert.AreEqual(1000, speed.TotalMetresPerSecond);
        Assert.AreEqual(3600, speed.TotalKilometresPerHour);
    }

}

}