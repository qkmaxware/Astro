using System;
using System.Collections.Generic;

namespace Astro.Remote.Services {

public class AlignmentStar {
    public string Name;
    public double J2000Ra;
    public double J2000Dec;
    public override string ToString() => Name;
}

public static class AlignmentStarCatalogue {

    public static IEnumerable<AlignmentStar> Stars => stars;

    private static List<AlignmentStar> stars = new List<AlignmentStar> {
        new AlignmentStar { Name = "Betelgeuse", J2000Ra = 88.7917, J2000Dec = 7.4069 },
    };
}

}