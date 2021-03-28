using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
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
    private static string ScriptQuery => BaseUrl + "/sim-script";
    private static RateLimiter blocker = new RateLimiter(5, TimeSpan.FromSeconds(1));

    private static string sendGet(HttpClient web, UriBuilder builder) {
        var path = builder.Uri.ToString();
        var task = web.GetStringAsync(path);
        task.Wait();
        var response = task.Result;
        return response;
    }

    private static IEnumerable<DeepSpaceEntity> sendQuery(string url, Dictionary<string, string>? uri_parametres = null) {
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
            // Query and clean up response to get just the XML
            var response = blocker.Invoke(() => sendGet(web, builder));
            Regex rgx = new Regex(":+data:+");
            var parts = rgx.Split(response ?? string.Empty); // Jump to the data section

            // Parse XML votable output to data structure
            var deserializer = new VOTableDeserializer(); 
            var votable = deserializer.Deserialize(new StringReader(parts[parts.Length - 1].TrimStart()));

            // Convert VOTable to Simbad.Entry
            var epoch = string.IsNullOrEmpty(votable.Epoch) ? Moment.J2000 : Moment.FromJulianYear(int.Parse(votable.Epoch.Substring(1)));
            foreach (var table in votable.Tables) {
                for (var row = 0; row < table.RowCount; row++) {
                    // Compute distance
                    var distanceUnit = table.SelectFirstNonEmpty(row, "Distance:unit", "Distance_unit");
                    string distanceString = table.SelectFirstNonEmpty(row, "Distance:distance",  "Distance_distance");
                    Distance? dist = null;
                    if (!string.IsNullOrEmpty(distanceString)) {
                        var distance = double.Parse(distanceString);
                        dist = distanceUnit switch {
                            "kpc" => Distance.Kiloparsecs(distance),
                            "Mpc" => Distance.Megaparsecs(distance),
                            _ => Distance.Parsecs(distance)
                        };
                    }

                    // Compute celestial coordinates
                    var raString = table.SelectFirstNonEmpty(row, "RA_d", "RA(d)");
                    Angle? ra = null;
                    if (!string.IsNullOrEmpty(raString)) {
                        var ra_deg = double.Parse(raString);
                        var ra_hr = ra_deg * (24.0d / 360.0d);
                        ra = Angle.Hours(ra_hr);
                    }  
                    var lastString = table.SelectFirstNonEmpty(row, "DEC_d", "DEC(d)");
                    Angle? decl = null;
                    if (!string.IsNullOrEmpty(lastString)) {   
                        var lat_deg = double.Parse(lastString);
                        decl = Angle.Degrees(lat_deg);
                    }

                    // Compute motion parametres
                    var raPropString = table[row, "PMRA"];
                    var decPropString = table[row, "PMDEC"];
                    ProperMotion? motion = null;
                    if (!string.IsNullOrEmpty(raPropString) && !string.IsNullOrEmpty(decPropString)) {
                        var ra_prop_motion = double.Parse(raPropString);                      // unit mas.yr-1
                        var dec_prop_motion = double.Parse(decPropString);                    // unit mas.yr-1
                        var year = TimeSpan.FromDays(365.25);
                        var ra_hour_angles_per_year = (ra_prop_motion / (3600 * 1000)) / 15;  // Convert mas to hrangle
                        var dec_degrees_per_year = dec_prop_motion / (3600 * 1000);           // Convert mas to degrees
                        motion = new ProperMotion(
                            raRate: new RateOfChange<Angle>(Angle.Hours(ra_hour_angles_per_year), year),
                            decRate: new RateOfChange<Angle>(Angle.Degrees(dec_degrees_per_year), year)
                        );
                    }
                    
                    // Compute object type

                    // Return
                    yield return new DeepSpaceEntity(
                        name:       table[row, "MAIN_ID"],
                        // type:       type,
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

    // Script reference
    // http://simbad.u-strasbg.fr/simbad/sim-fscript
    // http://simbad.u-strasbg.fr/guide/sim-fscript.htx
    // http://simbad.u-strasbg.fr/simbad/sim-display?data=otypes
    private static string makeQueryString (string query) {
        var script = @"votable vot1 {
	MAIN_ID
    OTYPE
	RA(d)
	DEC(d)
	PMRA
	PMDEC
	Distance
}
votable open vot1
set limit 0
query " + query;
        return script;
    }

    public static IEnumerable<DeepSpaceEntity> FromScript(string script) {
        return sendQuery(ScriptQuery, new Dictionary<string, string>{
            {"script", script},
        });
    }

    public static IEnumerable<DeepSpaceEntity> WithIdentifier(string id) {
        var query = makeQueryString($"id {id}");
        return FromScript(query);
    }

    public static IEnumerable<DeepSpaceEntity> WithinDistance(Distance distance) {
        return WithCriteria ($"Distance.unit='pc' & Distance.distance={distance.TotalParsecs}");
    }

    public static IEnumerable<DeepSpaceEntity> WithCriteria(string query) {
        return FromScript($"sample {query}");
    }

    public static IEnumerable<DeepSpaceEntity> FromCatalogue(string catalogue) {
        return FromScript($"cat {catalogue}");
    }

    public static IEnumerable<DeepSpaceEntity> MessierCatalogue() {
        return FromCatalogue("messier");
    }

    public static IEnumerable<DeepSpaceEntity> NewGeneralCatalogue() {
        return FromCatalogue("ngc");
    }

}

}