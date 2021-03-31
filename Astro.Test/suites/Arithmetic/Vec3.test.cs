using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class Vec3Test {
    [TestMethod]
    public void TestConstructor() {
        Vec3 vec = new Vec3(4,3,2);

        Assert.AreEqual(4, vec.X);
        Assert.AreEqual(3, vec.Y);
        Assert.AreEqual(2, vec.Z);
    }

    [TestMethod]
    public void TestEquals() {
        Vec3 a = new Vec3(4,3,2);
        Vec3 b = new Vec3(4,3,2);
        Vec3 c = new Vec3(2,3,4);

        Assert.AreEqual(true, a.Equals(b));
        Assert.AreEqual(false, a.Equals(c));
    }

    [TestMethod]
    public void TestLength() {
        Vec3 vec = new Vec3(4,3,2);

        Assert.AreEqual(29, vec.SqrLength.Value);
        Assert.AreEqual(Math.Sqrt(29), vec.Length.Value);
    }

    [TestMethod]
    public void TestUnit() {
        Vec3 unit = new Vec3(1, 0, 0);
        Vec3 vec = new Vec3(4, 0, 0);

        Assert.AreEqual(16, vec.SqrLength.Value);
        Assert.AreEqual(unit, vec.Normalized);
    }

    [TestMethod]
    public void TestFlipped() {
        Vec3 vec = new Vec3(4, -2, 0);
        var f = vec.Flipped;

        Assert.AreEqual(-4, f.X.Value);
        Assert.AreEqual(2, f.Y.Value);
        Assert.AreEqual(0, f.Z.Value);
    }

    [TestMethod]
    public void TestSqrDistance() {
        Vec3 a = new Vec3(1,0,0);
        Vec3 b = new Vec3(4,0,0);

        Assert.AreEqual(9, Vec3.SqrDistance(a,b).Value);
    }

    [TestMethod]
    public void TestDistance() {
        Vec3 a = new Vec3(1,0,0);
        Vec3 b = new Vec3(4,0,0);

        Assert.AreEqual(3, Vec3.Distance(a,b).Value);
    }

    [TestMethod]
    public void TestDot() {
        Vec3 a = new Vec3(1,2,3);
        Vec3 b = new Vec3(6,5,4);

        Assert.AreEqual(28, Vec3.Dot(a,b).Value);
    }

    [TestMethod]
    public void TestCross() {
        Assert.AreEqual(true, Vec3.Cross(Vec3.I,Vec3.J).Equals(Vec3.K));
        Assert.AreEqual(true, Vec3.Cross(Vec3.J,Vec3.I).Equals(-Vec3.K)); //

        Assert.AreEqual(true, Vec3.Cross(Vec3.K,Vec3.J).Equals(-Vec3.I));
        Assert.AreEqual(true, Vec3.Cross(Vec3.J,Vec3.K).Equals(Vec3.I)); //

        Assert.AreEqual(true, Vec3.Cross(Vec3.I,Vec3.K).Equals(-Vec3.J));
        Assert.AreEqual(true, Vec3.Cross(Vec3.K,Vec3.I).Equals(Vec3.J));
    }

    [TestMethod]
    public void TestAdd() {
        Vec3 a = new Vec3(1,2,3);
        Vec3 b = new Vec3(3,2,1);

        var r = a + b;

        Assert.AreEqual(4, r.X.Value);
        Assert.AreEqual(4, r.Y.Value);
        Assert.AreEqual(4, r.Z.Value);
    }

    [TestMethod]
    public void TestSub() {
        Vec3 a = new Vec3(1,2,3);
        Vec3 b = new Vec3(3,2,1);

        var r = a - b;

        Assert.AreEqual(-2, r.X.Value);
        Assert.AreEqual(0, r.Y.Value);
        Assert.AreEqual(2, r.Z.Value);
    }

    [TestMethod]
    public void TestNegate() {
        Vec3 a = new Vec3(1,2,3);

        Assert.AreEqual(a.Flipped, -a);
    }

    [TestMethod]
    public void TestScale() {
        Vec3 a = new Vec3(1,2,4);

        var b = 2d * a;
        var c = a * 3d;
        var d = a / 2d;

        Assert.AreEqual(2, b.X.Value);
        Assert.AreEqual(4, b.Y.Value);
        Assert.AreEqual(8, b.Z.Value);

        Assert.AreEqual(3, c.X.Value);
        Assert.AreEqual(6, c.Y.Value);
        Assert.AreEqual(12, c.Z.Value);

        Assert.AreEqual(0.5, d.X.Value);
        Assert.AreEqual(1, d.Y.Value);
        Assert.AreEqual(2, d.Z.Value);
    }
}

}