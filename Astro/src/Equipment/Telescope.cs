using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Equipment {

/// <summary>
/// Telescope description
/// </summary>
public class Telescope {
	// https://skyandtelescope.org/observing/stargazers-corner/simple-formulas-for-the-telescope-owner/
	public Length Aperture {get; set;}
	public Length FocalLength {get; set;}
	public Scientific FocalRatio => FocalLength.TotalMillimetres() / Aperture.TotalMillimetres();
	public Lens? OcularLens;
	
	public Scientific Magnification => OcularLens == null ? 1 : FocalLength.TotalMillimetres() / OcularLens.FocalLength.TotalMillimetres();

	public Angle AparentFieldOfView {get; set;}
	public Angle TrueFieldOfView => AparentFieldOfView.ScaleBy(1/Magnification);

	public Telescope(Length aperture, Length focalLength, Angle aFov) {
		this.Aperture = aperture;
		this.FocalLength = focalLength;
		this.AparentFieldOfView = aFov;
	}
	
}

}