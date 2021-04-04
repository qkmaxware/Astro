using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Astro.Dynamics;

namespace Qkmaxware.Astro.Tests {

[TestClass]
public class ReferenceFrameTest {

    [TestMethod]
    public void TestHierarchy() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), Angle.Zero, Angle.Zero);
        childB.ParentReferenceFrame = parent;

        Assert.AreEqual(root, childA.RootReferenceFrame);
        Assert.AreEqual(parent, ReferenceFrame.FindSharedParent(childA, childB));
    }

    [TestMethod]
    public void TestNestedPosition() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), Angle.Zero, Angle.Zero);
        childB.ParentReferenceFrame = parent;

        var zero = new Vec3<Distance>(Distance.Zero, Distance.Zero, Distance.Zero);

        var global = childA.LocalToGlobalPosition(zero);
        Assert.AreEqual(new Vec3<Distance>(Distance.Kilometres(4), Distance.Kilometres(0), Distance.Kilometres(0)), global);
        global = childA.LocalToGlobalPosition(new Vec3<Distance>(Distance.Kilometres(3), Distance.Kilometres(1), Distance.Kilometres(2)));
        Assert.AreEqual(new Vec3<Distance>(Distance.Kilometres(7), Distance.Kilometres(1), Distance.Kilometres(2)), global);
    
        global = childB.LocalToGlobalPosition(zero);
        Assert.AreEqual(new Vec3<Distance>(Distance.Kilometres(2), Distance.Kilometres(0), Distance.Kilometres(0)), global);
        global = childB.LocalToGlobalPosition(new Vec3<Distance>(Distance.Zero, Distance.Kilometres(1), Distance.Zero));
        Assert.AreEqual(2 - 0.707107, global.X.TotalKilometres, 0.001);
        Assert.AreEqual(0.707107, global.Y.TotalKilometres, 0.001);
        Assert.AreEqual(0, global.Z.TotalKilometres, 0.001);

        global = zero;
        var local = childA.GlobalToLocalPosition(global);
        Assert.AreEqual(-4, local.X.TotalKilometres, 0.001);
        Assert.AreEqual(0, local.Y.TotalKilometres, 0.001);
        Assert.AreEqual(0, local.Z.TotalKilometres, 0.001);
    }

    [TestMethod]
    public void TestFrameToFrame() {
        // Create hierarchy
        var root = new ReferenceFrame();

        var parent = new ReferenceFrame();
        parent.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        parent.ParentReferenceFrame = root;

        var childA = new ReferenceFrame(); // Shifted child
        childA.Position = new Arithmetic.Vec3<Distance>(Distance.Kilometres(2), Distance.Zero, Distance.Zero);
        childA.ParentReferenceFrame = parent;

        var childB = new ReferenceFrame(); // Rotated child
        childB.Rotation = Arithmetic.Quat.YawPitchRoll(Angle.Degrees(45), Angle.Zero, Angle.Zero);
        childB.ParentReferenceFrame = parent;

        var local = new Vec3<Distance>(Distance.Zero, Distance.Kilometres(1), Distance.Zero);
        var global = childB.LocalToPositionInFrame(local, childA);
        Assert.AreEqual(-0.707107 - 2, global.X.TotalKilometres, 0.001);
        Assert.AreEqual(0.707107, global.Y.TotalKilometres, 0.001);
        Assert.AreEqual(0, global.Z.TotalKilometres, 0.001);
    }

}

}