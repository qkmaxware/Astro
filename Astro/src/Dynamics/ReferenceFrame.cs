using System;
using System.Collections.Generic;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro.Dynamics {
    
/// <summary>
/// Reference frames define a coordinate system
/// </summary>
public class ReferenceFrame {
    /// <summary>
    /// Reference frame in which this frame is defined
    /// </summary>
    public ReferenceFrame? ParentReferenceFrame {get; set;}
    /// <summary>
    /// Reference frame at the root of the tree
    /// </summary>
    public ReferenceFrame RootReferenceFrame {
        get {
            var root = this;
            while (root.ParentReferenceFrame != null) {
                root = root.ParentReferenceFrame;
            }
            return root;
        }   
    }
    /// <summary>
    /// Rotation of this reference frame relative to its parent frame
    /// </summary>
    public Quat Rotation {get; set;} = Quat.Identity;
    /// <summary>
    /// Position of this reference frame relative to its parent frame
    /// </summary>
    public Vec3<Distance> Position {get; set;} = new Vec3<Distance>(Distance.Zero, Distance.Zero, Distance.Zero);

    /// <summary>
    /// Find a parent frame that is shared between two reference frames
    /// </summary>
    /// <param name="one">first reference frame</param>
    /// <param name="two">second reference frame</param>
    /// <returns>shared parent frame if one exists</returns>
    public static ReferenceFrame? FindSharedParent(ReferenceFrame one, ReferenceFrame two) {
        // Create paths
        List<ReferenceFrame> pathOne = new List<ReferenceFrame>();
        List<ReferenceFrame> pathTwo = new List<ReferenceFrame>();

        // Fill paths
        var frame = one;
        while (frame != null) {
            pathOne.Add(frame);
            frame = frame.ParentReferenceFrame;
        }

        frame = two;
        while (frame != null) {
            pathTwo.Add(frame);
            frame = frame.ParentReferenceFrame;
        }

        // Iterate from the end (root) forwards towards the leaf (reference frame)
        for (var i = 0; i < pathOne.Count; i++) {
            var frameOne = pathOne[i];
            var idx = pathTwo.IndexOf(frameOne);
            if (idx >= 0) {
                return frameOne;
            }
        }
        return null;
    }

    /// <summary>
    /// Search for a parent frame which extends from the given type
    /// </summary>
    /// <param name="frame">frame to start searching at</param>
    /// <typeparam name="T">type of frame to search for</typeparam>
    /// <returns>frame of the given type</returns>
    public static T? FindParentOfType<T> (ReferenceFrame frame) where T:ReferenceFrame {
        var parent = frame.ParentReferenceFrame;
        while (parent != null) {
            if (parent is T)
                return (T) parent;
            parent = parent.ParentReferenceFrame;
        }
        return default(T);
    }

    /// <summary>
    /// Covert a position within this frame of reference to a position within another frame
    /// </summary>
    /// <param name="position">position in local space</param>
    /// <param name="frame">other frame</param>
    /// <returns>position in other frame</returns>
    public Vec3<Distance> LocalToPositionInFrame(Vec3<Distance> position, ReferenceFrame frame) {
        var root = FindSharedParent(this, frame); // will always share the "null" frame at least

        // Get position in "parent" frame
        var parent = this;
        while (parent != root && parent != null) {
            position = position.Rotate(parent.Rotation) + parent.Position;
            parent = parent.ParentReferenceFrame;
        }

        // Get position in "other" frame
        List<ReferenceFrame> reversePath = new List<ReferenceFrame>();
        parent = frame;
        while (parent != root && parent != null) {
            reversePath.Add(parent);
            parent = parent.ParentReferenceFrame;
        }

        for (var i = 0; i < reversePath.Count; i++) {
            var current = reversePath[reversePath.Count - 1 - i];
            position = position.Rotate(current.Rotation.Conjugate) - current.Position;
        }

        return position;
    }

    /// <summary>
    /// Convert a position within this frame of reference to a position in "global" space
    /// </summary>
    /// <param name="local">position within this frame</param>
    /// <returns>position in the global frame</returns>
    public Vec3<Distance> LocalToGlobalPosition (Vec3<Distance> local) {
        var position = local;

        // Move up to shared "null parent space"
        var parent = this;
        while (parent != null) {
            position = position.Rotate(parent.Rotation) + parent.Position;
            parent = parent.ParentReferenceFrame;
        }
        return position;
    }
    
    /// <summary>
    /// Convert a position in "global" space to a position within this frame of reference
    /// </summary>
    /// <param name="global">position in "global" space</param>
    /// <returns>position in local space</returns>
    public Vec3<Distance> GlobalToLocalPosition(Vec3<Distance> global) {
        var position = global;
        
        // Get all nodes to the global node
        List<ReferenceFrame> reversePath = new List<ReferenceFrame>();
        var parent = this;
        while (parent != null) {
            reversePath.Add(parent);
            parent = parent.ParentReferenceFrame;
        }

        // Traverse in reverse order
        for (var i = 0; i < reversePath.Count; i++) {
            var frame = reversePath[reversePath.Count - 1 - i];
            position = position.Rotate(frame.Rotation.Conjugate) - frame.Position;
        }

        return position;
    }   

    /// <summary>
    /// Get the position of this reference frame relative to another frame of reference
    /// </summary>
    /// <param name="frame">other frame of reference</param>
    /// <returns>position relative to the input frame</returns>
    public Vec3<Distance> GetPositionRelativeTo(ReferenceFrame frame) {
        return LocalToPositionInFrame(
            new Vec3<Distance>(Distance.Zero, Distance.Zero, Distance.Zero),
            frame
        );
    }
}

}