using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Qkmaxware.Astro;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Astro.Query;

namespace Qkmaxware.Astro.Apps.Remote.Data {

public class CataloguedObject {
    public KeplerianEntity SatelliteData;
    public KeplerianEntity StationData;
    public KeplerianEntity PlanetData;
    public DeepSpaceEntity StarData;
    public DeepSpaceEntity GalaxyData;
    public DeepSpaceEntity OtherDeepSpaceData;

    public override string ToString() {
        return SatelliteData?.Name ?? StationData?.Name ?? PlanetData?.Name ?? StarData?.Name ?? GalaxyData?.Name ?? OtherDeepSpaceData?.Name;
    }

    public double getRa() {
        if (StarData != null) {
            return (double)StarData.Coordinates.RightAscension.TotalDegrees();
        } else if (GalaxyData != null) {
            return (double)GalaxyData.Coordinates.RightAscension.TotalDegrees();
        } else if (OtherDeepSpaceData != null) {
            return (double)OtherDeepSpaceData.Coordinates.RightAscension.TotalDegrees();
        } else {
            return 0;
        }
    }

    public double getDec() {
        if (StarData != null) {
            return (double)StarData.Coordinates.Declination.TotalDegrees();
        } else if (GalaxyData != null) {
            return (double)GalaxyData.Coordinates.Declination.TotalDegrees();
        }  else if (OtherDeepSpaceData != null) {
            return (double)OtherDeepSpaceData.Coordinates.Declination.TotalDegrees();
        } else {
            return 0;
        }
    }

    public bool HasRaAndDec() {
        if (StarData != null) {
            return StarData.Coordinates != null;
        } else if (GalaxyData != null) {
            return GalaxyData.Coordinates != null;
        } else if (OtherDeepSpaceData != null) {
            return OtherDeepSpaceData.Coordinates != null;
        } else {
            return false;
        }
    }
    
}

public abstract class NetworkCatalogue {
    public abstract IEnumerable<CataloguedObject> Search(string name);
}

public class CelestrakSatelliteCatalogue : NetworkCatalogue {
    public override IEnumerable<CataloguedObject> Search(string name) {
        return Celestrak.ActiveSatellites().Where(entity => entity.Name.StartsWith(name)).Select(entity => new CataloguedObject{ SatelliteData = entity });
    }
}

public class CelestrakStationCatalogue : NetworkCatalogue {
    public override IEnumerable<CataloguedObject> Search(string name) {
        return Celestrak.SpaceStations().Where(entity => entity.Name.StartsWith(name)).Select(entity => new CataloguedObject{ StationData = entity });
    }
}

public class SimbadIdCatalogue : NetworkCatalogue {
    public override IEnumerable<CataloguedObject> Search(string name) {
        return Simbad.WithIdentifier(name).Select(entity => {
            if (entity.Class.Contains("*")) {
                return new CataloguedObject{ StarData = entity };
            } else if (entity.Class.Contains("G")) {
                return new CataloguedObject{ GalaxyData = entity };
            } else {
                return new CataloguedObject{ OtherDeepSpaceData = entity };
            }
        });
    }
}

}