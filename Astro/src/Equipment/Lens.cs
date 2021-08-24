using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Equipment {

/// <summary>
/// Abstract base class for lenses
/// </summary>
public abstract class Lens {
	public abstract Length FocalLength { get; }
}

}