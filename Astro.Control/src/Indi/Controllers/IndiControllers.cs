using System.Linq;

namespace Qkmaxware.Astro.Control {

public enum SlewRate {
    Guide, Centering, Find, Max
}

public enum Direction {
    None, North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest
}

/// <summary>
/// Controller to slew a GOTO telescope mount
/// </summary>
public class IndiTelescopeController {
    private IndiDevice device;
    private string goto_j2000_property = "EQUATORIAL_COORD";
    private string goto_jnow_property = "EQUATORIAL_EOD_COORD";
    private string manual_nav_property = "HORIZONTAL_COORD";

    public bool IsAligned {get; private set;} = false;

    public IndiTelescopeController(IndiDevice device) {
        this.device = device;
    }

    private T getOrMake<T>(string prop) where T:IndiValue, new() {
        if (device.Properties.HasProperty(prop)) {
            return (T)device.Properties[prop];
        } else {
            var t = new T();
            t.Name = prop;
            return t;
        }
    }

    public void RefreshProperties() {
        this.device.RefreshProperties();
    }

    public void Connect() {
        IndiVector<IndiSwitchValue> vector;
        if (device.Properties.HasProperty(IndiStandardProperties.Connection)) {
            vector = (IndiVector<IndiSwitchValue>)device.Properties[IndiStandardProperties.Connection];
            vector.SwitchTo("CONNECT");
        } else {
            vector = new IndiVector<IndiSwitchValue>(IndiStandardProperties.Connection);
            vector.Add(new IndiSwitchValue {
                Name = "CONNECT",
                IsOn = true
            });
            vector.Add(new IndiSwitchValue {
                Name = "DISCONNECT",
                IsOn = false
            });
        }
        this.device.UpdateProperty(vector.Name, vector);
        this.device.RefreshProperties();
    }

    public void Disconnect() {
        IndiVector<IndiSwitchValue> vector;
        if (device.Properties.HasProperty(IndiStandardProperties.Connection)) {
            vector = (IndiVector<IndiSwitchValue>)device.Properties[IndiStandardProperties.Connection];
            vector.SwitchTo("DISCONNECT");
        } else {
            vector = new IndiVector<IndiSwitchValue>(IndiStandardProperties.Connection);
            vector.Add(new IndiSwitchValue {
                Name = "CONNECT",
                IsOn = false
            });
            vector.Add(new IndiSwitchValue {
                Name = "DISCONNECT",
                IsOn = true
            });
        }
        this.device.UpdateProperty(vector.Name, vector);
        this.device.RefreshProperties();
    }

    private string mode;
    private void setMode(string mode) {
        var vector = new IndiVector<IndiSwitchValue>(IndiStandardProperties.TelescopeOnCoordinateSet);
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
        if (this.device.Properties.HasProperty(IndiStandardProperties.TelescopeSlewRate)) {
            var vector = (IndiVector<IndiSwitchValue>)this.device.Properties[IndiStandardProperties.TelescopeSlewRate];
            var index = (int)(((int)rate / 3f) * (vector.Count - 1)); 
            vector.SwitchTo(index);
            device.UpdateProperty(vector.Name, vector);
        } else {
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
    }

    public void ClearMotion() {
        Rotate(Direction.None);
    }

    public void Rotate(Direction motion) {
        var vector = getOrMake<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeMotionWestEast);
        if (vector.Count < 1) {
            vector.Add(new IndiSwitchValue {
                Name = "MOTION_WEST",
                IsOn = false,
            });
            vector.Add(new IndiSwitchValue {
                Name = "MOTION_EAST",
                IsOn = false,
            });
        }
        vector.GetSwitch("MOTION_WEST").IsOn = motion == Direction.West || motion == Direction.NorthWest || motion == Direction.SouthWest;
        vector.GetSwitch("MOTION_EAST").IsOn = motion == Direction.East || motion == Direction.NorthEast || motion == Direction.SouthEast;
        device.UpdateProperty(vector.Name, vector);

        vector = getOrMake<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeMotionNorthSouth);
        if (vector.Count < 1) {
            vector.Add(new IndiSwitchValue {
                Name = "MOTION_NORTH",
                IsOn = false,
            });
            vector.Add(new IndiSwitchValue {
                Name = "MOTION_SOUTH",
                IsOn = false,
            });
        }
        vector.GetSwitch("MOTION_NORTH").IsOn = motion == Direction.North || motion == Direction.NorthEast || motion == Direction.NorthWest;
        vector.GetSwitch("MOTION_SOUTH").IsOn = motion == Direction.South || motion == Direction.SouthEast || motion == Direction.SouthWest;
        device.UpdateProperty(vector.Name, vector);
    }
}

}