using Xamarin.Essentials;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astro.Remote.Services {

public static class Position {

    public static Location GetLocation(bool forceRefresh = false) {
        if (!forceRefresh) {
            var cached = getCachedLocation();
            cached.Wait();
            if (cached.Result != null)
                return cached.Result;
        }

        var real = getCurrentLocation();
        real.Wait();
        if (real.Result != null)
            return real.Result;
        
        return null;
    }

    private static async Task<Location> getCachedLocation() {
        try {
            return await Geolocation.GetLastKnownLocationAsync();
        }
        catch (FeatureNotSupportedException fnsEx){
            // Handle not supported on device exception
            throw fnsEx;
        }
        catch (FeatureNotEnabledException fneEx){
            // Handle not enabled on device exception
            throw fneEx;
        }
        catch (PermissionException pEx){
            // Handle permission exception
            throw pEx;
        }
        catch (Exception ex){
            // Unable to get location
            throw ex;
        }
    }
      
    private static async Task<Location> getCurrentLocation() {
        try {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var cts = new CancellationTokenSource();
            return await Geolocation.GetLocationAsync(request, cts.Token);
        }
        catch (FeatureNotSupportedException fnsEx){
            // Handle not supported on device exception
            throw fnsEx;
        }
        catch (FeatureNotEnabledException fneEx){
            // Handle not enabled on device exception
            throw fneEx;
        }
        catch (PermissionException pEx){
            // Handle permission exception
            throw pEx;
        }
        catch (Exception ex){
            // Unable to get location
            throw ex;
        }
    }

}

}