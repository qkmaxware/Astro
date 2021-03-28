using System;

namespace Qkmaxware.Astro.Query {

/// <summary>
/// Astronomical catalogued entity 
/// </summary>
public class DeepSpaceEntity : CataloguedEntity {
    /// <summary>
    /// Entity coordinates in the celestial sphere
    /// </summary>
    public CelestialCoordinate Coordinates {get; private set;}

    /// <summary>
    /// Create a new astronomical entity
    /// </summary>
    /// <param name="name">entity name</param>
    /// <param name="type">entity type</param>
    /// <param name="epoch">reference timestamp</param>
    /// <param name="coordinate">entity coordinates</param>
    public DeepSpaceEntity(string name, Moment epoch, CelestialCoordinate coordinate) : base(name, epoch) {
        this.Coordinates = coordinate;
    }

    /// <summary>
    /// Compute the position of the entity at a specific moment in time
    /// </summary>
    /// <param name="moment">moment in time</param>
    /// <returns>coordinates at time</returns>
    public CelestialCoordinate? PositionAt (Moment moment) {
        var difference = moment - this.Epoch;
        return PositionAfter(difference);
    }

    /// <summary>
    /// Compute the position of the entity by advancing it forward in time using its stored proper motion
    /// </summary>
    /// <param name="time">time to advance</param>
    /// <returns>coordinates at the new time</returns>
    public CelestialCoordinate? PositionAfter(TimeSpan time) {
        if (this.Coordinates == null || this.Coordinates.RightAscension == null || this.Coordinates.Declination == null)
            return null;                // No coordinates
        if (this.Coordinates.ProperMotion == null)
            return this.Coordinates;    // No motion
        
        var raRate = this.Coordinates.ProperMotion.RightAscensionRate.Amount.TotalHours 
                    / this.Coordinates.ProperMotion.RightAscensionRate.Duration.TotalSeconds;

        var decRate = this.Coordinates.ProperMotion.DeclinationRate.Amount.TotalDegrees 
                    / this.Coordinates.ProperMotion.DeclinationRate.Duration.TotalSeconds;

        var timeSpan = time.TotalSeconds;

        var newRa = this.Coordinates.RightAscension.Add(Angle.Hours(raRate * timeSpan));
        var newDec = this.Coordinates.Declination.Add(Angle.Degrees(decRate * timeSpan));

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
    public Distance? DistanceTo(DeepSpaceEntity other) {
        if (this.Coordinates.SolDistance == null || this.Coordinates.Declination == null || this.Coordinates.RightAscension == null)
            return null;
        if (other.Coordinates.SolDistance == null || other.Coordinates.Declination == null || other.Coordinates.RightAscension == null)
            return null;

        var R1 = this.Coordinates.SolDistance.TotalKilometres;
        var A1 = this.Coordinates.RightAscension.TotalRadians;
        var D1 = this.Coordinates.Declination.TotalRadians;

        var R2 = other.Coordinates.SolDistance.TotalKilometres;
        var A2 = other.Coordinates.RightAscension.TotalRadians;
        var D2 = other.Coordinates.Declination.TotalRadians;

        var x1 = R1 * Math.Cos(A1) * Math.Cos(D1);
        var y1 = R1 * Math.Sin(A1) * Math.Cos(D1);
        var z1 = R1 * Math.Sin(D1);

        var x2 = R2 * Math.Cos(A2) * Math.Cos(D2);
        var y2 = R2 * Math.Sin(A2) * Math.Cos(D2);
        var z2 = R2 * Math.Sin(D2);

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