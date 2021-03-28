using System.Linq;

namespace Qkmaxware.Astro.Control {

public enum SlewRate {
    Guide, Centering, Find, Max
}

/// <summary>
/// Controller to slew a GOTO telescope mount
/// </summary>
public class IndiTelescopeController {
    private IndiDevice device;
    private string track_mode_property;
    private string goto_j2000_property = "EQUATORIAL_COORD";
    private string goto_jnow_property = "EQUATORIAL_EOD_COORD";
    private string manual_nav_property = "HORIZONTAL_COORD";

    public bool IsAligned {get; private set;} = false;

    public IndiTelescopeController(IndiDevice device) {
        this.device = device;
        this.track_mode_property = this.device.Properties.Where(prop => prop.Key.Contains("TRACK_MODE")).First().Key;
    }

    public void RefreshProperties() {
        this.device.RefreshProperties();
    }

    private string mode;
    private void setMode(string mode) {
        var vector = new IndiVector<IndiSwitchValue>() {
            Name = "ON_COORD_SET"
        };
        vector.Add(new IndiSwitchValue {
            Name="SLEW",
            IsOn = "SLEW" == mode,
        });
        vector.Add(new IndiSwitchValue {
            Name="TRACK",
            IsOn = "TRACK" == mode,
        });
        vector.Add(new IndiSwitchValue {
            Name="SYNC",
            IsOn = "SYNC" == mode,
        });
        this.mode = mode;
        System.Console.WriteLine(vector.CreateNewElement().ToString());
        this.device.UpdateProperty("ON_COORD_SET", vector);
    }
    private void slewNext() {
        if (mode != "SLEW")
            setMode("SLEW");
    }
    private void trackNext() {
        if (mode != "TRACK")
            setMode("TRACK");
    }
    private void syncNext() {
        if (mode != "SYNC")
            setMode("SYNC");
    }

    // TODO
    /*public void Align(IAlignmentModel model) {
        // Send Sync command
        syncNext();
        // Send current position
        var ra = model.JNowRa();
        var dec = model.JNowDec();
        // TODO 
        // Mark aligned
        this.IsAligned = true;
    }

    public void Track(double ra, double dec) {
        if (!IsAligned) {
            throw new Exception("Unable to track object until the telescope is aligned");
        }
        trackNext();
        var property = goto_jnow_property or goto_j2000_property;
        var vector = new IndiVector<IndiNumberValue>(property);
        vector.Add(new IndiNumberValue{
            Name = "RA",
            Value = ra,
            Min = -90,
            Max = 90,
            Step = 0.01,
        });
        vector.Add(new IndiNumberValue{
            Name = "DEC",
            Value = dec,
            Min = -180,
            Max = 180,
            Step = 0.01,
        });
        device.UpdateProperty(property, vector);
    }*/

    public void ResetRotation() {
        Goto(0, 0);    
    }

    public void Goto(double alt, double az) {
        var vector = new IndiVector<IndiNumberValue>();
        vector.Name = manual_nav_property;
        vector.Add(new IndiNumberValue {
            Name = "ALT",
            Value = alt,
            Min = -90,
            Max = 90,
            Step = 0.01,
        });
        vector.Add(new IndiNumberValue {
            Name = "AZ",
            Value = az,
            Min = -180,
            Max = 180,
            Step = 0.01,
        });
        slewNext();
        device.UpdateProperty(manual_nav_property, vector);
    }

    public void SetSlewRate(SlewRate rate) {
        var vector = new IndiVector<IndiSwitchValue>("TELESCOPE_SLEW_RATE");
        vector.Add(new IndiSwitchValue {
            Name = "SLEW_GUIDE",
            IsOn = rate == SlewRate.Guide,
        });
        vector.Add(new IndiSwitchValue {
            Name = "SLEW_CENTERING",
            IsOn = rate == SlewRate.Centering,
        });
        vector.Add(new IndiSwitchValue {
            Name = "SLEW_FIND",
            IsOn = rate == SlewRate.Find,
        });
        vector.Add(new IndiSwitchValue {
            Name = "SLEW_MAX",
            IsOn = rate == SlewRate.Max,
        });
        device.UpdateProperty(vector.Name, vector);
    }

    public void RotateWest(bool rotate) {
        var vector = new IndiVector<IndiSwitchValue>("TELESCOPE_MOTION_WE");
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_WEST",
            IsOn = rotate,
        });
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_EAST",
            IsOn = false,
        });
        device.UpdateProperty(vector.Name, vector);
    }

    public void RotateEast(bool rotate) {
        var vector = new IndiVector<IndiSwitchValue>("TELESCOPE_MOTION_WE");
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_WEST",
            IsOn = false,
        });
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_EAST",
            IsOn = rotate,
        });
        device.UpdateProperty(vector.Name, vector);
    }

    public void RotateNorth(bool rotate) {
        var vector = new IndiVector<IndiSwitchValue>("TELESCOPE_MOTION_NS");
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_NORTH",
            IsOn = rotate,
        });
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_SOUTH",
            IsOn = false,
        });
        device.UpdateProperty(vector.Name, vector);
    }

    public void RotateSouth(bool rotate) {
        var vector = new IndiVector<IndiSwitchValue>("TELESCOPE_MOTION_NS");
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_NORTH",
            IsOn = false,
        });
        vector.Add(new IndiSwitchValue {
            Name = "MOTION_SOUTH",
            IsOn = rotate,
        });
        device.UpdateProperty(vector.Name, vector);
    }
}

}