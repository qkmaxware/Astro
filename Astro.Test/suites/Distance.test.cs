using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class DistanceTest {

    [TestMethod]
    public void TestMetres ()  {
        var distance = Distance.Metres(1000000);
        Assert.AreEqual(1000, distance.TotalKilometres, 0.001);
        Assert.AreEqual(1000000, distance.TotalMetres, 0.001);
        Assert.AreEqual(1.057e-10, distance.TotalLightYears, 0.001);
        Assert.AreEqual(3.24078e-11, distance.TotalParsecs, 0.001);
    }

    [TestMethod]
    public void TestKilometers ()  {
        var distance = Distance.Kilometres(1000);
        Assert.AreEqual(1000, distance.TotalKilometres, 0.001);
        Assert.AreEqual(1000000, distance.TotalMetres, 0.001);
        Assert.AreEqual(1.057e-10, distance.TotalLightYears, 0.001);
        Assert.AreEqual(3.24078e-11, distance.TotalParsecs, 0.001);
    }

    [TestMethod]
    public void TestLightYears ()  {
        var distance = Distance.Lightyears(1.057e-10);
        Assert.AreEqual(1000, distance.TotalKilometres, 0.001);
        Assert.AreEqual(1000000, distance.TotalMetres, 1);
        Assert.AreEqual(1.057e-10, distance.TotalLightYears, 0.001);
        Assert.AreEqual(3.24078e-11, distance.TotalParsecs, 0.001);
    }

    [TestMethod]
    public void TestParsecs ()  {
        var distance = Distance.Parsecs(3.24078e-11);
        Assert.AreEqual(1000, distance.TotalKilometres, 0.001);
        Assert.AreEqual(1000000, distance.TotalMetres, 1);
        Assert.AreEqual(1.057e-10, distance.TotalLightYears, 0.001);
        Assert.AreEqual(3.24078e-11, distance.TotalParsecs, 0.001);
    }

}

}