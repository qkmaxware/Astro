using System;
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

    public bool IsAligned {get; private set;} = false;

    public IndiTelescopeController(IndiDevice device) {
        this.device = device;
    }

    private T getProp<T>(string prop) where T:IndiValue, new() {
        if (device.Properties.HasProperty(prop)) {
            var t = device.Properties[prop];
            if (t is T) {
                return (T)device.Properties[prop];
            } else {
                throw new System.ArgumentException($"Device property '{prop}' is not of type {typeof(T).Name}");
            }
        } else {
            throw new System.ArgumentException($"Device is missing required property '{prop}'");
        }
    }

    public void RefreshProperties() {
        this.device.RefreshProperties();
    }

    public void Connect() {
        IndiVector<IndiSwitchValue> vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.Connection);
        var connect = vector.GetSwitch("CONNECT");
        if (connect != null && !connect.IsOn) {
            // Only connect if not already connected
            foreach (var toggle in vector) {
                toggle.Value = toggle == connect;
            }
            this.device.ChangeRemoteProperty(vector.Name, vector);
            this.device.RefreshProperties();
        }
    }

    public void Disconnect() {
        IndiVector<IndiSwitchValue> vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.Connection);
        var disconnect = vector.GetSwitch("CONNECT");
        if (disconnect != null && !disconnect.IsOn) {
            // Only disconnect if not already disconnected
            foreach (var toggle in vector) {
                toggle.Value = toggle == disconnect;
            }
            this.device.ChangeRemoteProperty(vector.Name, vector);
            this.device.RefreshProperties();
        }
    }

    private string mode;
    private void setMode(string mode) {
        var vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeOnCoordinateSet);;
        this.mode = mode;
        vector.SwitchTo((toggle) => toggle.Name == mode);
        this.device.ChangeRemoteProperty("ON_COORD_SET", vector);
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
        throw new System.NotImplementedException();
    }

    public void SetLocation(double lat, double lon, double alt) {
        var vector = getProp<IndiVector<IndiNumberValue>>(IndiStandardProperties.GeographicCoordinate);
        vector.GetItemWithName("LAT").Value = lat;
        vector.GetItemWithName("LONG").Value = lon;
        vector.GetItemWithName("ELEV").Value = Math.Max(0, alt);
        device.ChangeRemoteProperty(vector.Name, vector);
    }

    public void SetSlewRate(SlewRate rate) {
        var vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeSlewRate);
        var index = (int)(((int)rate / 3f) * (vector.Count - 1)); 
        vector.SwitchTo(index);
        device.ChangeRemoteProperty(vector.Name, vector);
    }

    public void ClearMotion() {
        Rotate(Direction.None);
    }

    public void Rotate(Direction motion) {
        var vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeMotionWestEast);
        vector.GetSwitch("MOTION_WEST").Value = motion == Direction.West || motion == Direction.NorthWest || motion == Direction.SouthWest;
        vector.GetSwitch("MOTION_EAST").Value = motion == Direction.East || motion == Direction.NorthEast || motion == Direction.SouthEast;
        device.ChangeRemoteProperty(vector.Name, vector);

        vector = getProp<IndiVector<IndiSwitchValue>>(IndiStandardProperties.TelescopeMotionNorthSouth);
        vector.GetSwitch("MOTION_NORTH").Value = motion == Direction.North || motion == Direction.NorthEast || motion == Direction.NorthWest;
        vector.GetSwitch("MOTION_SOUTH").Value = motion == Direction.South || motion == Direction.SouthEast || motion == Direction.SouthWest;
        device.ChangeRemoteProperty(vector.Name, vector);
    }
}

}