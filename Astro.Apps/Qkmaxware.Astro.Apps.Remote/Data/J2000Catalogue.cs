using System;
using System.Collections.Generic;
using System.Linq;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Astro.Query;
using Qkmaxware.Measurement;

namespace Qkmaxware.Astro.Apps.Remote.Data {

public class J2000Catalogue : NetworkCatalogue {

    
    private IEnumerable<KeplerianEntity> Planets() => Qkmaxware.Astro.Constants.Planets.AtJ2000();

    public override IEnumerable<CataloguedObject> Search(string name) {
        return Planets()
        .Where( planet => planet.Name.IndexOf(name, 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
        .Select( planet => 
            new CataloguedObject {
                PlanetData = planet
            }
        );
    }
}

}