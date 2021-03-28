using Qkmaxware.Astro.Dynamics;

namespace Qkmaxware.Astro.Query {
    
/// <summary>
/// An entity that obeys keplerian motion
/// </summary>
public class KeplerianEntity : CataloguedEntity {
    /// <summary>
    /// Orbital elements for this object's orbit
    /// </summary>
    public OrbitalElements OrbitalElements {get; private set;}

    /// <summary>
    /// Create a new keplerian entity
    /// </summary>
    /// <param name="name">name of the entity</param>
    /// <param name="epoch">epoch related to when the orbit is relative to</param>
    /// <param name="elements">orbital elements</param>
    public KeplerianEntity(string name, Moment epoch, OrbitalElements elements) : base(name, epoch) {
        this.OrbitalElements = elements;
    }
}
    
}