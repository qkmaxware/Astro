using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class MomentTest {
    [TestMethod]
    public void JulianDays() {
        Moment moment = new DateTime(2020, 7, 30, 0, 0, 0, DateTimeKind.Utc);
        
        // Verify
        Assert.AreEqual(2459060.5, moment.JulianDay, 0.01);
    }

    [TestMethod]
    public void J2000() {
        var julian = Moment.J2000;

        // Verify
        Assert.AreEqual(2000, julian.Year);
        Assert.AreEqual(Month.Jan, julian.Month);
        Assert.AreEqual(1, julian.Day);
    }

    [TestMethod]
    public void GreenwichMeanSiderealTime() {
        Moment moment = new DateTime(year: 2002, month: (int)Month.Nov, day: 25, hour: 0, minute: 0, second: 0, DateTimeKind.Utc);
        var sidereal = moment.GreenwichMeanSiderealTime();

        // Verify
        Assert.AreEqual(2452603.5, moment.JulianDay);
        Assert.AreEqual(04, sidereal.Hours);     
        Assert.AreEqual(15, sidereal.Minutes);     
        Assert.AreEqual(04, sidereal.Seconds);     
    }

    [TestMethod]
    public void TestTimeDifference() {
        Moment t1 = new DateTime(year: 2021, month: (int)Month.Jan, day: 1);
        Moment t2 = new DateTime(year: 2021, month: (int)Month.Jan, day: 7);

        var duration = t2 - t1;
        Assert.AreEqual(6, duration.TotalDays);
    }
}

}