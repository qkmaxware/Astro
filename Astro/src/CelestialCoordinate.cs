using System;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro {

/// <summary>
/// Coordinate for an entity in the celestial sphere
/// </summary>
public class CelestialCoordinate {
    /// <summary>
    /// Length from the Sol system
    /// </summary>
    public Length? SolDistance {get; private set;}
    /// <summary>
    /// Right ascension angle
    /// </summary>
    public Angle? RightAscension {get; private set;}
    /// <summary>
    /// Horizon declaration angle
    /// </summary>
    public Angle? Declination {get; private set;}

    /// <summary>
    /// Proper motion representing the rate of change of this coordinate over time
    /// </summary>
    public ProperMotion? ProperMotion {get; private set;}

    /// <summary>
    /// Create a new celestial coordinate
    /// </summary>
    /// <param name="distance">Length from the sun</param>
    /// <param name="ra">right ascension</param>
    /// <param name="dec">declination</param>
    /// <param name="motion">optional proper motion</param>
    public CelestialCoordinate(Length? distance, Angle? ra, Angle? dec, ProperMotion? motion = null) {
        this.SolDistance = distance;
        this.RightAscension = ra;
        this.Declination = dec;
        this.ProperMotion = motion;
    }
}

}