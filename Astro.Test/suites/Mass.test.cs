using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class MassTest {

    [TestMethod]
    public void TestKilogram() {
        var mass = Mass.Kilograms(1000);
        Assert.AreEqual(1000, mass.TotalKilograms);
        Assert.AreEqual(1000000, mass.TotalGrams);
    }

    [TestMethod]
    public void TestGram() {
        var mass = Mass.Grams(1000000);
        Assert.AreEqual(1000, mass.TotalKilograms);
        Assert.AreEqual(1000000, mass.TotalGrams);
    }

}

}