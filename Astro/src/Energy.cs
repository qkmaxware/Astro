using System;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {
    
/// <summary>
/// Measure of energy
/// </summary>
public class Energy {
    private double value;

    /// <summary>
    /// Total joules of energy
    /// </summary>
    public double TotalJoules => value;

    private Energy(double J) {
        this.value = J;
    }

    /// <summary>
    /// Create an energy measured in joules
    /// </summary>
    /// <param name="J">joules</param>
    /// <returns>energy</returns>
    public static Energy Joules(double J) {
        return new Energy(J);
    }
}

}