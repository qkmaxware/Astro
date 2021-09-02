using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Qkmaxware.Astro.Constants;
using Qkmaxware.Astro.IO;

namespace Qkmaxware.Astro.Query {

public static class Celestrak {
    public static readonly string ActiveSatellitesUrl = "https://celestrak.com/NORAD/elements/active.txt";
    public static readonly string SpaceStationsUrl = "https://celestrak.com/NORAD/elements/stations.txt";

    private static StreamReader downloadTleText(string url) {
        var cookies = new CookieContainer();
        using (var handler = new HttpClientHandler { CookieContainer = cookies, UseCookies = true })
        using (var client = new System.Net.Http.HttpClient(handler)) 
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

            var task = client.GetStreamAsync(url);
            task.Wait();
            return new StreamReader(task.Result);
        }
    }

    private static IEnumerable<KeplerianEntity> readTle(string url) {
        var source = downloadTleText(url);
        var serializer = new TwoLineElementDeserializer();
        return serializer.Deserialize(Planets.EarthJ2000.Mass, source);
    }

    public static IEnumerable<KeplerianEntity> SpaceStations() {
        return readTle(SpaceStationsUrl);
    }

    public static IEnumerable<KeplerianEntity> ActiveSatellites() {
        return readTle(ActiveSatellitesUrl);
    }
}

}