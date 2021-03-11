using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Coordinate for an entity in the celestial sphere
/// </summary>
public class CelestialCoordinate {
    /// <summary>
    /// Distance from the Sol system
    /// </summary>
    public Distance SolDistance {get; private set;}
    /// <summary>
    /// Right ascension angle
    /// </summary>
    public RightAscension RightAscension {get; private set;}
    /// <summary>
    /// Horizon declaration angle
    /// </summary>
    public Declination Declination {get; private set;}

    /// <summary>
    /// Proper motion representing the rate of change of this coordinate over time
    /// </summary>
    public ProperMotion ProperMotion {get; private set;}

    /// <summary>
    /// Create a new celestial coordinate
    /// </summary>
    /// <param name="distance">distance from the sun</param>
    /// <param name="ra">right ascension</param>
    /// <param name="dec">declination</param>
    /// <param name="motion">optional proper motion</param>
    public CelestialCoordinate(Distance distance, RightAscension ra, Declination dec, ProperMotion motion = null) {
        this.SolDistance = distance;
        this.RightAscension = ra;
        this.Declination = dec;
        this.ProperMotion = motion;
    }
}

/// <summary>
/// Astronomical catalogued entity 
/// </summary>
public class AstronomicalEntity {
    /// <summary>
    /// Entity name
    /// </summary>
    public string Name {get; private set;}
    /// <summary>
    /// Entity reference timestamp
    /// </summary>
    public Moment Epoch {get; private set;}
    /// <summary>
    /// Entity coordinates in the celestial sphere
    /// </summary>
    public CelestialCoordinate Coordinates {get; private set;}
    private static double deg2rad = (Math.PI * 2.0) / 360.0;

    /// <summary>
    /// Create a new astronomical entity
    /// </summary>
    /// <param name="name">entity name</param>
    /// <param name="epoch">reference timestamp</param>
    /// <param name="coordinate">entity coordinates</param>
    public AstronomicalEntity(string name, Moment epoch, CelestialCoordinate coordinate) {
        this.Name = name;
        this.Epoch = epoch;
        this.Coordinates = coordinate;
    }

    /// <summary>
    /// Compute the position of the entity at a specific moment in time
    /// </summary>
    /// <param name="moment">moment in time</param>
    /// <returns>coordinates at time</returns>
    public CelestialCoordinate PositionAt (Moment moment) {
        var difference = moment - this.Epoch;
        return PositionAfter(difference);
    }

    /// <summary>
    /// Compute the position of the entity by advancing it forward in time using its stored proper motion
    /// </summary>
    /// <param name="time">time to advance</param>
    /// <returns>coordinates at the new time</returns>
    public CelestialCoordinate PositionAfter(TimeSpan time) {
        if (this.Coordinates == null)
            return null;                // No coordinates
        if (this.Coordinates.ProperMotion == null)
            return this.Coordinates;    // No motion
        
        var raRate = this.Coordinates.ProperMotion.RightAscensionRate.Amount.TotalHours 
                    / this.Coordinates.ProperMotion.RightAscensionRate.Duration.TotalSeconds;

        var decRate = this.Coordinates.ProperMotion.DeclinationRate.Amount.TotalDegrees 
                    / this.Coordinates.ProperMotion.DeclinationRate.Duration.TotalSeconds;

        var timeSpan = time.TotalSeconds;

        var newRa = this.Coordinates.RightAscension.AddHours(raRate * timeSpan);
        var newDec = this.Coordinates.Declination.AddDegrees(decRate * timeSpan);

        return new CelestialCoordinate (
            distance: this.Coordinates.SolDistance,
            ra: newRa,
            dec: newDec,
            motion: this.Coordinates.ProperMotion
        );
    }

    /// <summary>
    /// Compute the distance between this entity and another using their current coordinates
    /// </summary>
    /// <param name="other">other entity</param>
    /// <returns>Distance between entities</returns>
    public Distance DistanceTo(AstronomicalEntity other) {
        var R1 = this.Coordinates.SolDistance.TotalKilometres;
        var A1 = this.Coordinates.RightAscension.TotalDegrees;
        var D1 = this.Coordinates.Declination.TotalDegrees;

        var R2 = other.Coordinates.SolDistance.TotalKilometres;
        var A2 = other.Coordinates.RightAscension.TotalDegrees;
        var D2 = other.Coordinates.Declination.TotalDegrees;

        var x1 = R1 * Math.Cos(deg2rad * A1) * Math.Cos(deg2rad * D1);
        var y1 = R1 * Math.Sin(deg2rad * A1) * Math.Cos(deg2rad * D1);
        var z1 = R1 * Math.Sin(deg2rad * D1);

        var x2 = R2 * Math.Cos(deg2rad * A2) * Math.Cos(deg2rad * D2);
        var y2 = R2 * Math.Sin(deg2rad * A2) * Math.Cos(deg2rad * D2);
        var z2 = R2 * Math.Sin(deg2rad * D2);

        return Distance.Kilometres(
            Math.Sqrt(
                  (x2 - x1) * (x2 - x1)
                + (y2 - y1) * (y2 - y1)
                + (z2 - z1) * (z2 -z1)
            )
        );
    }
}

}