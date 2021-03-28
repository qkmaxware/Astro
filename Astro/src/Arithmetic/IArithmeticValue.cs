using System;

namespace Qkmaxware.Astro.Arithmetic {

/// <summary>
/// An object that can be added
/// </summary>
/// <typeparam name="T">operator type to add</typeparam>
public interface IAddable<T> {
    T Add (T other);
}

/// <summary>
/// An object that can be subtracted
/// </summary>
/// <typeparam name="T">operator type to subtract</typeparam>
public interface ISubtractable<T> {
    T Subtract (T other);
}

/// <summary>
/// An object that can be divided
/// </summary>
/// <typeparam name="T">operator type to divide</typeparam>
public interface IDividable<T> {
    T Divide (T other);
}

/// <summary>
/// An object that can be multiplied
/// </summary>
/// <typeparam name="T">operator type to multiply</typeparam>
public interface IMultiplyable<T> {
    T Multiply (T other);
}

/// <summary>
/// An object that can be scaled by a scalar quantity
/// </summary>
/// <typeparam name="T">type of result</typeparam>
public interface IScaleable<T> {
    T Scale (double scalar);
}

/// <summary>
/// An object that can be square rooted
/// </summary>
/// <typeparam name="T">type after square root</typeparam>
public interface ISqrt<T> {
    T Sqrt();
}

/// <summary>
/// Object that supports all basic arithmetic operators
/// </summary>
/// <typeparam name="T">operator type</typeparam>
public interface IArithmeticValue<T> : IAddable<T>, ISubtractable<T>, IMultiplyable<T>, IDividable<T>, IScaleable<T>, ISqrt<T> {}

}