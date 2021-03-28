using System;
using Qkmaxware.Astro.Arithmetic;

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
        // Keep rotating the vector to match the reference frame, no moving is required
        var root = FindSharedParent(this, frame);
        Quat toRoot = Quat.Identity;
        Quat fromRoot = Quat.Identity;

        ReferenceFrame path = this;
        while (path != root && path != null) {
            toRoot = path.Rotation * toRoot;
        }
        path = frame;
        while (path != root && path != null) {
            fromRoot = path.Rotation * fromRoot;
        }

        return (fromRoot.Conjugate * toRoot) * this.Velocity;
    }

    /// <summary>
    /// Get the orbital elements for an orbit of this object around its parent entity
    /// </summary>
    /// <returns>orbital elements</returns>
    public OrbitalElements? GetOrbitRelativeToParent() {
        // Input args
        var parent = ParentEntity;
        if (parent == null)
            return null;
        
        var position = this.GetPositionRelativeTo(parent).Convert(x => (Real)x.TotalMetres);
        var velocity = this.GetVelocityRelativeTo(parent).Convert(x => (Real)x.TotalMetresPerSecond);
        var distance = position.Length;
        var speed = velocity.Length;
        var M = parent.Mass;
        var m = this.Mass;

        // Cartesian state vector to orbital element conversion
        var H = Vec3<Real>.Cross(position, velocity);
        var h = H.Length;
        var up = new Vec3<Real>(0,0,1);
        var N = Vec3<Real>.Cross(up, H);
        var n = N.Length;

        var E = (Vec3<Real>.Cross(velocity,H) / M.μ) - (position / distance);
        var e = E.Length;

        var energy = (speed * speed)/2 - M.μ/distance;

        double a; double p;
        if (Math.Abs(e - 1.0) > double.Epsilon) {
            a = -M.μ / (2 * energy);
            p = a * (1 - e * e);
        } else {
            p = (h * h) / M.μ;
            a = double.PositiveInfinity;
        }

        double i = Math.Acos(H.Z / h);

        double eps = double.Epsilon;
        double Omega; double w;
        if (Math.Abs(i) < eps) {
            Omega = 0; // For non-inclined orbits, this is undefined, set to 0 by convention
            if (Math.Abs(e) < eps) {
                w = 0; // For circular orbits, place periapsis at ascending node by convention
            }
            else {
                w = Math.Acos(E.X / e); 
            }
        } else {
            Omega = Math.Acos(N.X / n);
            if (N.Y < 0) {
                Omega = (2 * Math.PI) - Omega;
            }

            w = Math.Acos(Vec3<Real>.Dot(N, E) / (n * e));
        }

        double nu;
        if (Math.Abs(e) < eps) {
            if (Math.Abs(i) < eps) {
                nu = Math.Acos(position.X / distance);
                if (velocity.X > 0) {
                    nu = (2 * Math.PI) - nu;
                }
            } else {
                nu = Math.Acos(Vec3<Real>.Dot(N,position) / (n * distance));
                if (Vec3<Real>.Dot(N,velocity) > 0) {
                    nu = (2 * Math.PI) - nu;
                }
            }
        } else {
            if (E.Z < 0) {
                w = (2 * Math.PI) - w;
            }

            nu = Math.Acos(Vec3<Real>.Dot(E, position) / (e * distance));
            if (Vec3<Real>.Dot(position,velocity) < 0) {
                nu = (2 * Math.PI) - nu;
            }
        }

        // Set variables
        return new OrbitalElements(
            a:              Distance.Metres(a),
            i:              Angle.Radians(i),
            e:              e,
            Ω:              Angle.Radians(Omega),
            ω:              Angle.Radians(w),
            anomalyType:    AnomalyType.True,
            anomalyValue:   Angle.Radians(nu)
        );
    }
}

}