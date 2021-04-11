using System;
using System.Collections.Generic;
using System.IO;
using Qkmaxware.Astro.Dynamics;
using Qkmaxware.Astro.Query;

namespace Qkmaxware.Astro.IO {

/// <summary>
/// Class for deserializing entities from two line element sets
/// </summary>
public class TwoLineElementDeserializer {

    /// <summary>
    /// Deserialize a TLE file such as those found on Celestrak
    /// </summary>
    /// <param name="parent">parent body mass for derivation of TLE orbit</param>
    /// <param name="reader">text reader to a TLE file</param>
    /// <returns>list of all two line element sets within the TLE file</returns>
    public IEnumerable<KeplerianEntity> Deserialize(Mass parent, TextReader reader) {
        var now = DateTime.Now;
        var nowYearPrefix = now.Year.ToString().Substring(0, 2);

        string? title_line = null;
        while ((title_line = reader.ReadLine()) != null) {
            string? line_1 = reader.ReadLine();
            string? line_2 = reader.ReadLine();
            if (line_1 == null || line_2 == null)
                break;

            // Parse line 1
            int line_1_number = (int)line_1[0];
            int catalog = int.Parse(line_1.Substring(2, 5));
            char @class = (char)line_1[7];
            int year = int.Parse(nowYearPrefix + line_1.Substring(18, 2));
            double dayOfYear = double.Parse(line_1.Substring(20, 12));
            DateTime epoch = DateTime.SpecifyKind(new DateTime(year, 1, 1), DateTimeKind.Utc).AddDays(dayOfYear);

            // Parse line 2
            int line_2_number = (int)line_2[0];
            double inc = double.Parse(line_2.Substring(8, 8));  // Degrees
            double ra = double.Parse(line_2.Substring(17, 8));  // Degrees
            double e = double.Parse("0." + line_2.Substring(26, 7));
            double pa = double.Parse(line_2.Substring(34, 8));  // Degrees
            double mean = double.Parse(line_2.Substring(43, 8));// Degrees

            var mu = parent.μ;
            double rev_day = double.Parse(line_2.Substring(52, 11));
            double a = Math.Pow(mu, 1.0/3.0) / Math.Pow((2 * Math.PI * rev_day) / 86400, 2.0/3.0); // μ^1/3 / mean_motion.toRadsPerSecond^2/3

            yield return new KeplerianEntity(
                name: title_line.Trim(),
                epoch: epoch,
                elements: new OrbitalElements(
                    a: Distance.Metres(a),
                    i: Angle.Degrees(inc),
                    e: e,
                    Ω: Angle.Degrees(ra),
                    w: Angle.Degrees(pa),
                    anomalyType: AnomalyType.Mean,
                    anomalyValue: Angle.Degrees(mean)
                )
            );
            /*yield return new TwoLineElementSet(
                title: title_line.Trim(),
                catalogNumber: catalog,
                @class: @class,
                epoch: epoch,
                elements: new OrbitalElements(
                    a: Distance.Metres(a),
                    i: Angle.Degrees(inc),
                    e: e,
                    Ω: Angle.Degrees(ra),
                    ω: Angle.Degrees(pa),
                    anomalyType: AnomalyType.Mean,
                    anomalyValue: Angle.Degrees(mean)
                )
            );*/
        }
    }

}

}