using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CommandLine;
using Qkmaxware.Astro;
using Qkmaxware.Astro.Query;
using System.Text.RegularExpressions;

namespace Qkmaxware.Astro.Cli {

    class Options {
        [Option("satellites", Default = false, HelpText = "Search the active satellite catalog")]
        public bool SearchSatellites {get; set;}
        [Option("stations", Default = false, HelpText = "Search the active station catalog")]
        public bool SearchStation {get; set;}
        [Option("id", Required = true, HelpText = "Regular expression filter")]
        public string id { get; set; }
    }

    class CelestrakCli {
        static void Main() {
            var args = System.Environment.GetCommandLineArgs();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => {
                var regex = new Regex(o.id);

                if (o.SearchSatellites) {
                    foreach (var entity in Celestrak.ActiveSatellites()) {
                        if (regex.IsMatch(entity.Name)) {
                            Console.WriteLine(entity.Name);
                            Console.WriteLine($"epoch: {entity.Epoch}, a: {entity.OrbitalElements.SemimajorAxis}, i: {entity.OrbitalElements.Inclination}, e: {entity.OrbitalElements.Eccentricity}, Ω: {entity.OrbitalElements.LongitudeOfAscendingNode}, w: {entity.OrbitalElements.ArgumentOfPeriapsis}, v: {entity.OrbitalElements.TrueAnomaly}, E: {entity.OrbitalElements.EccentricAnomaly}, M: {entity.OrbitalElements.MeanAnomaly}");
                            Console.WriteLine();
                        }
                    }
                }
                if (o.SearchStation) {
                    foreach (var entity in Celestrak.SpaceStations()) {
                        if (regex.IsMatch(entity.Name)) {
                            Console.WriteLine(entity.Name);
                            Console.WriteLine($"epoch: {entity.Epoch}, a: {entity.OrbitalElements.SemimajorAxis}, i: {entity.OrbitalElements.Inclination}, e: {entity.OrbitalElements.Eccentricity}, Ω: {entity.OrbitalElements.LongitudeOfAscendingNode}, w: {entity.OrbitalElements.ArgumentOfPeriapsis}, v: {entity.OrbitalElements.TrueAnomaly}, E: {entity.OrbitalElements.EccentricAnomaly}, M: {entity.OrbitalElements.MeanAnomaly}");
                            Console.WriteLine();
                        }
                    }
                }
            });
        }
    }
}
