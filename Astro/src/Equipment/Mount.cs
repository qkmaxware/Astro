using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Equipment {

/// <summary>
/// Abstract base class for equipment mounts
/// </summary>
/// <typeparam name="T">type of mounted equipment</typeparam>
public abstract class Mount<T> {
	public T? Device;
	public abstract void LookAt(Vec3<Length> position);
}

}