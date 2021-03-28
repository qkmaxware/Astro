using System;
using System.Threading;

namespace Qkmaxware.Astro.Query {

public class RateLimiter {

    private int queriesPerDuration;
    private TimeSpan duration;

    public int QueriesPerSecond => (int)Math.Floor(queriesPerDuration / duration.TotalSeconds);
    private DateTime timestamp = DateTime.Now;
    private int queriesSinceTimestamp = 0;

    private object key = new object();

    public RateLimiter(int queries, TimeSpan duration) {
        this.queriesPerDuration = Math.Max(queries, 1);
        this.duration = duration;
    }

    public T? Invoke<T>(Func<T> func) where T:class {
        T? value = default(T);
        Invoke(() => {
            value = func();
        });
        return value;
    }

    public void Invoke (Action action) {
        lock(key) {
            // Delay if we need to
            var now = DateTime.Now;
            var timeSinceTimestamp = now - timestamp;
            if (timeSinceTimestamp > duration) {
                // Reset
                timestamp = now;
                queriesSinceTimestamp = 1;
            } else {
                queriesSinceTimestamp++;
                if (queriesSinceTimestamp > queriesPerDuration) {
                    // Delay / reset
                    Thread.Sleep(duration);
                    timestamp = DateTime.Now;
                    queriesSinceTimestamp = 1;
                }
            }
                        
            // Call the api action
            action();
        }
    }
}

}