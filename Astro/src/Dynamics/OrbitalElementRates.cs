using System;
using Qkmaxware.Astro.Arithmetic;
using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Dynamics {

/// <summary>
/// Data class for storing rates of change in orbital elements
/// </summary>
public class OrbitalElementRates {
    public class CorrectionVariables {
        public double B {get; private set;}
        public double C {get; private set;}
        public double S {get; private set;}
        public double F {get; private set;}

        public CorrectionVariables() {}
        public CorrectionVariables(double b, double c, double s, double f) {
            this.B = b;
            this.C = c;
            this.S = s;
            this.F = f;
        }
    }

    public RateOfChange<Length> SemimajorAxis {get; private set;}
    public RateOfChange<Angle> Inclination {get; private set;}
    public RateOfChange<double> Eccentricity {get; private set;}
    public RateOfChange<Angle> LongitudeOfAscendingNode {get; private set;}
    public RateOfChange<Angle> ArgumentOfPeriapsis {get; private set;}
    
    public CorrectionVariables CorrectionTerms {get; private set;}

    public OrbitalElementRates(RateOfChange<Length> a, RateOfChange<Angle> i, RateOfChange<double> e, RateOfChange<Angle> Ω, RateOfChange<Angle> w) {
        this.SemimajorAxis = a;
        this.Inclination = i;
        this.Eccentricity = e;
        this.LongitudeOfAscendingNode = @Ω;
        this.ArgumentOfPeriapsis = @w;

        CorrectionTerms = new CorrectionVariables();
    }
}

}