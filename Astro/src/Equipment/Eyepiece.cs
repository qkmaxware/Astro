using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Equipment {

/// <summary>
/// Ocular eyepiece lens
/// </summary>
public class Eyepiece : Lens {
    private Length _focalLength;
    /// <summary>
    /// Focal length of the eyepiece
    /// </summary>
	public override Length FocalLength => _focalLength;

    public Eyepiece(Length focalLength) {
        this._focalLength = focalLength;
    }
}

}