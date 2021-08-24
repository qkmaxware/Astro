using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Dynamics {

/*
    var Earth = new PhysicsEntity {
        Mass = Mass.Kilogram(...),
    }
    var Selene = new PhysicsEntity {
        ParentFrame = Earth, 
        Mass = Mass.Kilogram(...),
        LocalPosition = new Vec3<Distance>(...),
        LocalVelocity = new Vec3<Speed>(...)
    }
    var orbit = Selene.GetOrbitRelativeToParent();
*/

/// <summary>
/// Astronomical entity governed by physics
/// </summary>
public class PhysicsEntity : ReferenceFrame {
    /// <summary>
    /// The parent physics entity 
    /// </summary>
    /// <returns>parent entity</returns>
    public PhysicsEntity? ParentEntity => ReferenceFrame.FindParentOfType<PhysicsEntity>(this);
    /// <summary>
    /// The mass of the physics entity
    /// </summary>
    /// <value>mass</value>
    public Mass Mass {get; set;} = Mass.Zero;
    /// <summary>
    /// The velocity of the physics entity relative to its parent frame of reference
    /// </summary>
    /// <returns>Velocity in the current frame</returns>
    public Vec3<Speed> Velocity {get; set;} = new Vec3<Speed>(Speed.Zero, Speed.Zero, Speed.Zero);

    /// <summary>
    /// Compute the velocity vector of this frame relative to another frame of reference
    /// </summary>
    /// <param name="frame">reference frame</param>
    /// <returns>velocity vector</returns>
    public Vec3<Speed> GetVelocityRelativeTo(ReferenceFrame frame) {
        return LocalToDirectionInFrame(this.Velocity, frame);
    }

    /*
    /// <summary>
    /// Copy the cartsian state of a given orbit
    /// </summary>
    /// <param name="elements">orbital elements to copy</param>
    private void CopyState(PhysicsEntity centralBody, OrbitalElements elements) {
        var parent = centralBody;
        if (parent != null) {
            var position = elements.CartesianPosition(parent.Mass);
            var velocity = elements.CartesianVelocity(parent.Mass);

            if (this.ParentReferenceFrame != null) {
                position = parent.LocalToPositionInFrame(position, this.ParentReferenceFrame);
                velocity = parent.LocalToDirectionInFrame(velocity, this.ParentReferenceFrame);
            } else {
                position = parent.LocalToGlobalPosition(position);
                velocity = parent.LocalToGlobalDirection(velocity);
            }

            this.Position = position;
            this.Velocity = velocity;
        }
    }
    */

    /// <summary>
    /// Get the orbital elements for an orbit of this object around its parent entity
    /// </summary>
    /// <returns>orbital elements</returns>
    public OrbitalElements? GetOrbitRelativeToParent() {
        // Input args
        var parent = ParentEntity;
        if (parent == null)
            return null;
        
        var position = this.GetPositionRelativeTo(parent);
        var velocity = this.GetVelocityRelativeTo(parent);

        // Set variables
        return new OrbitalElements(
            parent.Mass,
            position,
            velocity
        );
    }
}

}