using Microsoft.VisualStudio.TestTools.UnitTesting;
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

}

}