namespace Qkmaxware.Astro.Query {

/// <summary>
/// Base class for all catalogued entities
/// </summary>
public class CataloguedEntity {
    /// <summary>
    /// Entity name
    /// </summary>
    public string Name {get; private set;}
    /// <summary>
    /// Entity reference timestamp
    /// </summary>
    public Moment Epoch {get; private set;}

    public CataloguedEntity(string name, Moment epoch) {
        this.Name = name;
        this.Epoch = epoch;
    }
}

}