using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Qkmaxware.Astro.IO;

namespace Qkmaxware.Astro.Query {

/// <summary>
/// Interface to the Simbad Astronomical API
/// </summary>
public static class Simbad {

    private static readonly string BaseUrl = "http://simbad.u-strasbg.fr/simbad";
    private static string IdentifierQuery => BaseUrl + "/sim-id";
    private static string CoordinateQuery => BaseUrl + "/sim-coo";
    private static string ReferenceQuery => BaseUrl + "/sim-ref";
    private static string CriteraQuery => BaseUrl + "/sim-sam";
    private static RateLimiter blocker = new RateLimiter(5, TimeSpan.FromSeconds(1));

    private static string sendGet(HttpClient web, UriBuilder builder) {
        var path = builder.Uri.ToString();
        var task = web.GetStringAsync(path);
        task.Wait();
        var response = task.Result;
        return response;
    }

    private static IEnumerable<AstronomicalEntity> sendQuery(string url, Dictionary<string, string> uri_parametres = null) {
        // Configure URL parametres
        var builder = new UriBuilder(url);
        if (uri_parametres != null) {
            foreach (var kv in uri_parametres) {
                builder.AddParametre(kv.Key, kv.Value);
            }
        }
        builder.AddParametre("output.max", "50000");
        builder.AddParametre("frameN", "ICRS");
        builder.AddParametre("output.format", "votable");

        // Send web request
        using (var web = new HttpClient()) {
            var response = blocker.Invoke(() => sendGet(web, builder));
            var deserializer = new VOTableDeserializer();
            var votable = deserializer.Deserialize(new StringReader(response));

            // Convert VOTable to Simbad.Entry
            var epoch = string.IsNullOrEmpty(votable.Epoch) ? Moment.J2000 : Moment.FromJulianYear(int.Parse(votable.Epoch.Substring(1)));
            foreach (var table in votable.Tables) {
                for (var row = 0; row < table.RowCount; row++) {
                    // Compute distance
                    var distanceUnit = table[row, "Distance:unit"];
                    string distanceString = table[row, "Distance:distance"];
                    Distance dist = null;
                    if (!string.IsNullOrEmpty(distanceString)) {
                        var distance = double.Parse(distanceString);
                        dist = distanceUnit switch {
                            "kpc" => Distance.Kiloparsecs(distance),
                            "Mpc" => Distance.Megaparsecs(distance),
                            _ => Distance.Parsecs(distance)
                        };
                    }

                    // Compute celestial coordinates
                    var raString = table[row, "RA_d"];
                    RightAscension ra = null;
                    if (!string.IsNullOrEmpty(raString)) {
                        var ra_deg = double.Parse(raString);
                        var ra_hr = ra_deg * (24.0d / 360.0d);
                        ra = new RightAscension(ra_hr);
                    }  
                    var lastString = table[row, "DEC_d"];
                    Declination decl = null;
                    if (!string.IsNullOrEmpty(lastString)) {   
                        var lat_deg = double.Parse(lastString);
                        decl = new Declination(lat_deg);
                    }

                    // Compute motion parametres
                    var raPropString = table[row, "PMRA"];
                    var decPropString = table[row, "PMDEC"];
                    ProperMotion motion = null;
                    if (!string.IsNullOrEmpty(raPropString) && !string.IsNullOrEmpty(decPropString)) {
                        var ra_prop_motion = double.Parse(raPropString);                      // unit mas.yr-1
                        var dec_prop_motion = double.Parse(decPropString);                    // unit mas.yr-1
                        var year = TimeSpan.FromDays(365.25);
                        var ra_hour_angles_per_year = (ra_prop_motion / (3600 * 1000)) / 15;  // Convert mas to hrangle
                        var dec_degrees_per_year = dec_prop_motion / (3600 * 1000);           // Convert mas to degrees
                        motion = new ProperMotion(
                            raRate: new RateOfChange<RightAscension>(new RightAscension(ra_hour_angles_per_year), year),
                            decRate: new RateOfChange<Declination>(new Declination(dec_degrees_per_year), year)
                        );
                    }
                    

                    // Return
                    yield return new AstronomicalEntity(
                        name:       table[row, "MAIN_ID"],
                        epoch:      epoch,
                        coordinate: new CelestialCoordinate(
                            distance:   dist,
                            ra:         ra,
                            dec:        decl,
                            motion:     motion
                        )
                    ); 
                }
            }
        }
    }

    public static IEnumerable<AstronomicalEntity> WithIdentifier(string id) {
        var entries = sendQuery(IdentifierQuery, new Dictionary<string, string>() {
            {"Ident", id}
        });
        return entries;
    }

    public static IEnumerable<AstronomicalEntity> WithinDistance(Distance distance) {
        return WithCriteria ($"Distance.unit='pc' & Distance.distance={distance.TotalParsecs}");
    }

    public static IEnumerable<AstronomicalEntity> WithCriteria(string query) {
        return sendQuery(CriteraQuery, new Dictionary<string, string>() {
            {"Criteria", query},
            {"OutputMode", "LIST"},
            {"list.pmsel", "on"},
            {"list.cooN", "on"},
        });
    }

    public static IEnumerable<AstronomicalEntity> MessierCatalogue() {
        return sendQuery(ReferenceQuery, new Dictionary<string, string>{
            {"querymethod", "bib"},
            {"bibcode", "1850CDT..1784..227M"}
        });
    }

    public static IEnumerable<AstronomicalEntity> NewGeneralCatalogue() {
        return sendQuery(ReferenceQuery, new Dictionary<string, string>{
            {"querymethod", "bib"},
            {"bibcode", "1888MmRAS..49....1D"}
        });
    }

}

}