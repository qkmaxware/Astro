using System;

namespace Qkmaxware.Astro {

/// <summary>
/// Utility extention method for dealing with double precision values
/// </summary>
public static class DoubleUtilities {

    /// <summary>
    /// Wrap a floating point value between two end-points
    /// </summary>
    /// <param name="x">value</param>
    /// <param name="x_min">min</param>
    /// <param name="x_max">max</param>
    /// <returns>wrapped value</returns>
	public static double Wrap(this double x, double x_min, double x_max) {
        if (x >= x_min && x <= x_max)
            return x; // between the two values, don't edit it
        else
		    return x - (x_max - x_min) * Math.Floor( x / (x_max - x_min));
	}
    
    /// <summary>
    /// Clamp a value between two end-points
    /// </summary>
    /// <param name="x">value</param>
    /// <param name="x_min">min</param>
    /// <param name="x_max">max</param>
    /// <returns>clamped value</returns>
	public static double Clamp(this double x, double x_min, double x_max) {
		if (x < x_min)
			return x_min;
		else if (x > x_max)
			return x_max;
		else
			return x;
	}
    
    /// <summary>
    /// Clamp a value between 0 and 1
    /// </summary>
    /// <param name="x">value</param>
    /// <returns>clamped value</returns>
	public static double Clamp01(this double x) {
		return Clamp(x, 0.0d, 1.0d);
	}

    /// <summary>
    /// Remap a value from one range to another
    /// </summary>
    /// <param name="x">value to remap</param>
    /// <param name="originalMin">minimum value in original range</param>
    /// <param name="originalMax">maximum value in original range</param>
    /// <param name="newMin">minimum value in new range</param>
    /// <param name="newMax">maximum value in new range</param>
    /// <returns>scaled valued</returns>
    public static double Remap (this double x, double originalMin, double originalMax, double newMin, double newMax) {
        return (newMax - newMin) * (x - originalMin) / (originalMax - originalMin) + newMin;
    }

}

}