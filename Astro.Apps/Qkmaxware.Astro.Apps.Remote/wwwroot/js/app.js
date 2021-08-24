window.AstroRemote = (function(){

    function wrapGeolocationCallback() {
        return new Promise(function(resolve, reject) {
            try {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(resolve);
                } else {
                    throw "Geolocation is not supported";
                }
            } catch (e) {
                reject(e);
            }
        });
    }

    async function GetGpsLocation() {
        var position = await wrapGeolocationCallback();
        return position.coords;
    }

    return {
        GetGpsLocation
    };
})();