using Qkmaxware.Measurement;
using Qkmaxware.Numbers;

namespace Qkmaxware.Astro.Equipment {

/// <summary>
/// Barlow lens 
/// </summary>
public class Barlow : Lens {
	/// <summary>
	/// Magnification factor
	/// </summary>
	/// <value>mangification factor</value>
	public int Magnification {get; private set;}
	/// <summary>
	/// Focal length of this lens
	/// </summary>
	public override Length FocalLength => Attachment.FocalLength * Magnification;
	/// <summary>
	/// Additional lense attached to this barlow
	/// </summary>
	/// <value>lens</value>
	public Lens Attachment {get; private set;}
	
	
	public Barlow(int magnification, Lens attachment) {
		this.Magnification = magnification;
		this.Attachment = attachment;
	}
	
	/// <summary>
	/// Create a 2x barlow attached to the given lens
	/// </summary>
	/// <param name="attachment">attachment</param>
	/// <returns>lens</returns>
	public static Barlow x2 (Lens attachment) {
		return new Barlow(2, attachment);
	}
	/// <summary>
	/// Create a 3x barlow attached to the given lens
	/// </summary>
	/// <param name="attachment">attachment</param>
	/// <returns>lens</returns>
	public static Barlow x3 (Lens attachment) {
		return new Barlow(3, attachment);
	}
	/// <summary>
	/// Create a 5x barlow attached to the given lens
	/// </summary>
	/// <param name="attachment">attachment</param>
	/// <returns>lens</returns>
	public static Barlow x5 (Lens attachment) {
		return new Barlow(5, attachment);
	}
}

}