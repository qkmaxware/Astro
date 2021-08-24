using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Qkmaxware.Astro.Apps.Remote.Data {

public class GPSCoordinate {
    public double? altitude;
    public double? latitude;
    public double? longitude;
}

public class Position {
    public static async Task<GPSCoordinate> GetCurrent(IJSRuntime js) {
        return await js.InvokeAsync<GPSCoordinate>("AstroRemote.GetGpsLocation");
    }
}

}