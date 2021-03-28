using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Qkmaxware.Astro.IO;

namespace Qkmaxware.Astro.Query {

public static class Celestrak {
    private static string activeSatellitesUrl = "https://celestrak.com/NORAD/elements/active.txt";
    private static string spaceStationsUrl = "https://celestrak.com/NORAD/elements/stations.txt";

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
        return serializer.Deserialize(Mass.Earth, source);
    }

    public static IEnumerable<KeplerianEntity> SpaceStations() {
        return readTle(spaceStationsUrl);
    }

    public static IEnumerable<KeplerianEntity> ActiveSatellites() {
        return readTle(activeSatellitesUrl);
    }
}

}