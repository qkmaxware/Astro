using System;
using Qkmaxware.Astro.Arithmetic;

namespace Qkmaxware.Astro {
    
/// <summary>
/// Measure of force
/// </summary>
public class Force : IArithmeticValue<Force> {
    private double value;

    /// <summary>
    /// Total force measured in Newtons
    /// </summary>
    public double TotalNewtons => value;

    private Force(double newtons) {
        this.value = newtons;
    }

    /// <summary>
    /// Create an energy measured in Newtons
    /// </summary>
    /// <param name="n">newtons</param>
    /// <returns>force</returns>
    public static Force Joules(double n) {
        return new Force(n);
    }

    public Force Add(Force other){
        return new Force(this.value + other.value);
    }

    public Force Subtract(Force other) {
        return new Force(this.value - other.value);
    }

    public Force Multiply(Force other) {
        return new Force(this.value * other.value);
    }

    public Force Divide(Force other) {
        return new Force(this.value / other.value);
    }

    public Force Scale(double scalar) {
        return new Force(this.value * scalar);
    }

    public Force Sqrt() {
        return new Force(Math.Sqrt(this.value));
    }
}

}