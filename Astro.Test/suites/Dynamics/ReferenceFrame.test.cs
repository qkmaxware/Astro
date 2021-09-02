using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class ReferenceFrameTest {

    private Angle ZeroDegrees = Angle.Degrees(0);

    [TestMethod]
    public void TestHierarchy() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), ZeroDegrees, ZeroDegrees);
        childB.ParentReferenceFrame = parent;

        Assert.AreEqual(root, childA.RootReferenceFrame);
        Assert.AreEqual(parent, ReferenceFrame.FindSharedParent(childA, childB));
    }

    // TODO fix
    [TestMethod]
    public void TestNestedPosition() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), ZeroDegrees, ZeroDegrees);
        childB.ParentReferenceFrame = parent;

        var zero = new Vec3<Length>(Length.Zero, Length.Zero, Length.Zero);

        var global = childA.LocalToGlobalPosition(zero);
        Assert.AreEqual(new Vec3<Length>(Length.Kilometres(4), Length.Kilometres(0), Length.Kilometres(0)), global);
        global = childA.LocalToGlobalPosition(new Vec3<Length>(Length.Kilometres(3), Length.Kilometres(1), Length.Kilometres(2)));
        Assert.AreEqual(new Vec3<Length>(Length.Kilometres(7), Length.Kilometres(1), Length.Kilometres(2)), global);
    
        global = childB.LocalToGlobalPosition(zero);
        Assert.AreEqual(new Vec3<Length>(Length.Kilometres(2), Length.Kilometres(0), Length.Kilometres(0)), global);
        global = childB.LocalToGlobalPosition(new Vec3<Length>(Length.Zero, Length.Kilometres(1), Length.Zero));
        Assert.AreEqual(2 - 0.707107, (double)global.X.TotalKilometres(), 0.001);
        Assert.AreEqual(0.707107, (double)global.Y.TotalKilometres(), 0.001);
        Assert.AreEqual(0, (double)global.Z.TotalKilometres(), 0.001);

        global = zero;
        var local = childA.GlobalToLocalPosition(global);
        Assert.AreEqual(-4, (double)local.X.TotalKilometres(), 0.001);
        Assert.AreEqual(0, (double)local.Y.TotalKilometres(), 0.001);
        Assert.AreEqual(0, (double)local.Z.TotalKilometres(), 0.001);
    }

    [TestMethod]
    public void TestFrameToFrame() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Vec3<Length>(Length.Kilometres(2), Length.Zero, Length.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), ZeroDegrees, ZeroDegrees);
        childB.ParentReferenceFrame = parent;

        var local = new Vec3<Length>(Length.Zero, Length.Kilometres(1), Length.Zero);
        var global = childB.LocalToPositionInFrame(local, childA);
        Assert.AreEqual(-0.707107 - 2, (double)global.X.TotalKilometres(), 0.001);
        Assert.AreEqual(0.707107, (double)global.Y.TotalKilometres(), 0.001);
        Assert.AreEqual(0, (double)global.Z.TotalKilometres(), 0.001);
    }

}

}