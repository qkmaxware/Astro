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

    public bool IsPositioned {get; private set;}
    public bool IsOrientated {get; private set;}
    public bool IsAligned => IsPositioned && IsOrientated;

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

    public void SetTimeToClient() {
        var vector = this.getProp<IndiVector<IndiTextValue>>(IndiStandardProperties.LocalUtcTime);
        var time = DateTime.Now;
        if (vector.IsWritable) {
            vector.GetItemWithName("UTC").Value = time.ToUniversalTime().ToShortTimeString();
            vector.GetItemWithName("OFFSET").Value = TimeZoneInfo.Local.GetUtcOffset(time).TotalHours.ToString();
            device.ChangeRemoteProperty(vector.Name, vector);
        }
    }

    public void SetOrientation(double ra, double dec, bool J2000 = false) {
        var vector = this.getProp<IndiVector<IndiNumberValue>>(
            J2000 
            ? IndiStandardProperties.TelescopeJ2000EquatorialCoordinate 
            : IndiStandardProperties.TelescopeJNowEquatorialCoordinate
        );
        syncNext();
        vector.GetItemWithName("RA").Value = ra;
        vector.GetItemWithName("DEC").Value = dec;
        device.ChangeRemoteProperty(vector.Name, vector);
        this.IsOrientated = true;
    }

    public void SetLocation(double lat, double lon, double alt) {
        var vector = getProp<IndiVector<IndiNumberValue>>(IndiStandardProperties.GeographicCoordinate);
        vector.GetItemWithName("LAT").Value = lat;
        vector.GetItemWithName("LONG").Value = lon;
        vector.GetItemWithName("ELEV").Value = Math.Max(0, alt);
        device.ChangeRemoteProperty(vector.Name, vector);
        this.IsPositioned = true;
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

    public void Goto(double raDegrees, double decDegrees, bool J2000 = false) {
        var vector = this.getProp<IndiVector<IndiNumberValue>>(
            J2000 
            ? IndiStandardProperties.TelescopeJ2000EquatorialCoordinate 
            : IndiStandardProperties.TelescopeJNowEquatorialCoordinate
        );
        slewNext();
        vector.GetItemWithName("RA").Value = raDegrees;
        vector.GetItemWithName("DEC").Value = decDegrees;
        device.ChangeRemoteProperty(vector.Name, vector);
    }
}

}