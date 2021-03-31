using System;

namespace Qkmaxware.Astro.Arithmetic {
    
/// <summary>
/// Numeric quantity with no specific meaning
/// </summary>
public struct Real : IArithmeticValue<Real> {
    public double Value {get; private set;}

    public Real(double value) {
        this.Value = value;
    }

    public override bool Equals(object obj) {
        if (obj is Real real) {
            return this.Value == real.Value;
        } else 
            return base.Equals(obj);
    }

    public override int GetHashCode(){
        return Value.GetHashCode();
    }

    public override string ToString() {
        return Value.ToString();
    }

    public static implicit operator Real(double value) {
        return new Real(value);
    }

    public static implicit operator double(Real value) {
        return value.Value;
    }

    public Real Add(Real other) {
        return new Real(this.Value + other.Value);
    }

    public static Real operator + (Real a, Real b) {
        return a.Add(b);
    }

    public Real Divide(Real other) {
        return new Real(this.Value / other.Value);
    }

    public static Real operator / (Real a, Real b) {
        return a.Divide(b);
    }

    public Real Multiply(Real other) {
        return new Real(this.Value * other.Value);
    }

    public static Real operator * (Real a, Real b) {
        return a.Multiply(b);
    }

    public Real Scale(double scalar) {
        return new Real(this.Value * scalar);
    }

    public Real Sqrt() {
        return new Real(Math.Sqrt(this.Value));
    }

    public Real Subtract(Real other) {
        return new Real(this.Value - other.Value);
    }

    public static Real operator - (Real a, Real b) {
        return a.Subtract(b);
    }
}

}