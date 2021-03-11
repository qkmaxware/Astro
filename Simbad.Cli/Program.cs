using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CommandLine;
using Qkmaxware.Astro;
using Qkmaxware.Astro.Query;

namespace Qkmaxware.Astro.Cli {
    class Options {
        [Option("id", Required = false, HelpText = "Search by id")]
        public string id { get; set; }
        [Option("criteria", Required = false, HelpText = "Search by the given criteria")]
        public string criteria { get; set; }
        [Option('f', "format", Required = false, HelpText = "Format for output")]
        public OutputFormat format {get; set;} = OutputFormat.tabbed;
    }

    enum OutputFormat {
        tabbed,
        json,
        instance
    }

    class SimbadCli {
        public static void Main() {
            var args = System.Environment.GetCommandLineArgs();
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => {
                var list = Enumerable.Empty<AstronomicalEntity>();

                if (!string.IsNullOrEmpty(o.id)) {
                    list = list.Concat(Simbad.WithIdentifier(o.id));
                }
                if (!string.IsNullOrEmpty(o.criteria)) {
                    list = list.Concat(Simbad.WithCriteria(o.criteria));
                }

                switch (o.format) {
                    case OutputFormat.tabbed:
                        tabFormatted(list);
                        break;
                    case OutputFormat.json:
                        Console.WriteLine(
                            JsonSerializer.Serialize(list, new JsonSerializerOptions {
                                WriteIndented = true
                            })
                        );
                        break;
                    case OutputFormat.instance: 
                        instanceFormatted(list);
                        break;
                }
            });
        }

        private static void instanceFormatted(IEnumerable<AstronomicalEntity> entities) {
            Console.WriteLine($"new List<{nameof(AstronomicalEntity)}> {{");
            foreach (var entity in entities) {
                Console.WriteLine($"  new {nameof(AstronomicalEntity)} {{");
                Console.WriteLine($"    name: \"{entity.Name}\",");
                Console.WriteLine($"    epoch: Moment.FromJulianDays({entity.Epoch.JulianDay}),");
                if( entity.Coordinates != null) {
                Console.WriteLine($"    coordinate: new CelestialCoordinate(");
                if (entity.Coordinates.SolDistance != null) {
                Console.WriteLine($"      distance: Distance.Kilometres({entity.Coordinates.SolDistance.TotalKilometres}),");
                } else {
                Console.WriteLine($"      distance: null,");
                }
                if (entity.Coordinates.RightAscension != null) {
                Console.WriteLine($"      ra: new RightAscension({entity.Coordinates.RightAscension.TotalHours}),");
                } else {
                Console.WriteLine($"      ra: null,");  
                }
                if (entity.Coordinates.Declination != null) {
                Console.WriteLine($"      dec: new Declination({entity.Coordinates.Declination.TotalDegrees}),");
                } else {
                Console.WriteLine($"      dec: null,");  
                }
                if (entity.Coordinates.ProperMotion != null) {
                Console.WriteLine($"      motion: new ProperMotion {{");
                Console.WriteLine($"        raRate: new RateOfChange<RightAscension>(new RightAscension({entity.Coordinates.ProperMotion.RightAscensionRate.Amount.TotalHours}), TimeSpan.Parse(\"{entity.Coordinates.ProperMotion.RightAscensionRate.Duration}\")),");
                Console.WriteLine($"        decRate: new RateOfChange<Declination>(new Declination({entity.Coordinates.ProperMotion.DeclinationRate.Amount.TotalDegrees}), TimeSpan.Parse(\"{entity.Coordinates.ProperMotion.DeclinationRate.Duration}\")),");
                Console.WriteLine($"      }}");
                } else {
                Console.WriteLine($"      motion: null");   
                }
                Console.WriteLine($"    )");
                }
                Console.WriteLine($"  }},");
            }
            Console.WriteLine("}");
        }

        private static void tabFormatted(IEnumerable<AstronomicalEntity> entities) {
            foreach (var entity in entities) {
                Console.WriteLine($"{entity.Name} ({entity.Epoch.JulianDay})");
                Console.WriteLine($"  DIST: {entity.Coordinates?.SolDistance}");
                Console.WriteLine($"  RA:   {entity.Coordinates?.RightAscension} + {entity.Coordinates?.ProperMotion?.RightAscensionRate?.Amount}/{entity.Coordinates?.ProperMotion?.RightAscensionRate?.Duration}");
                Console.WriteLine($"  DEC:  {entity.Coordinates?.Declination} + {entity.Coordinates?.ProperMotion?.DeclinationRate?.Amount}/{entity.Coordinates?.ProperMotion?.DeclinationRate?.Duration}");
                Console.WriteLine();
            }
        }
    }
}
