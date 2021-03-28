using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Dynamic;
using System.Collections.Generic;

namespace Qkmaxware.Astro.Control {

public abstract class IndiDeviceMessage {
    public abstract void Process(IndiConnection connection);
    public abstract string EncodeXml();
}

public class IndiSetPropertyMessage : IndiDeviceMessage {
    public string DeviceName;
    public string PropertyName;
    public IndiValue PropertyValue;
    public IndiSetPropertyMessage(string property, IndiValue value) : this(null, property, value) {}
    public IndiSetPropertyMessage(string device, string property, IndiValue value) {
        this.DeviceName = device;
        this.PropertyName = property;
        this.PropertyValue = value;
    }
    public override void Process(IndiConnection connection) {
        /*
        if receive <setXXX> from Device
            change record of value and/or state for the specified Property
        */
        if (string.IsNullOrEmpty(DeviceName)) {
            // Set property for all devices
            foreach (var device in connection.Devices) {
                device.Value.Properties[PropertyName] = PropertyValue;
            }
        } else {
            var device = connection.GetDeviceByName(DeviceName);
            if (device != null) {
                // Set property on specific device
                device.Properties[PropertyName] = PropertyValue;
            }
        }
    }

    public override string EncodeXml() {
        return PropertyValue.CreateSetElement().ToString();
    }
}

public class IndiDefinePropertyMessage : IndiDeviceMessage {
    public string DeviceName;
    public string PropertyName;
    public IndiValue PropertyValue;
    public IndiDefinePropertyMessage(string device, string prop, IndiValue value) {
        this.DeviceName = device;
        this.PropertyName = prop;
        this.PropertyValue = value;
    }
    public override void Process(IndiConnection connection) {
        /*
        if receive <defProperty> from Device
            if first time to see this device=
                create new Device record
            if first time to see this device+name combination
                create new Property record within given Device
        */
        if (!string.IsNullOrEmpty(this.DeviceName) && !string.IsNullOrEmpty(this.PropertyName) && this.PropertyValue != null) {
            var device = connection.GetOrCreateDevice(this.DeviceName);
            device.Properties[this.PropertyName] = this.PropertyValue;
        }
    }

    public override string EncodeXml() {
        return PropertyValue.CreateDefinitionElement().ToString();
    }
}

public class IndiDeletePropertyMessage : IndiDeviceMessage {
    public string DeviceName;
    public string PropertyName;
    public bool DeleteAllProperties => string.IsNullOrEmpty(PropertyName);
    public string Timestamp;
    public string Message;
    public IndiDeletePropertyMessage(string device, string prop, string timestamp, string msg) {
        this.DeviceName = device;
        this.PropertyName = prop;
        this.Timestamp = timestamp;
        this.Message = msg;
    }

    public override void Process(IndiConnection connection) {
        /*
        if receive <delProperty> from Device
            if includes device= attribute
                if includes name= attribute
                    delete record for just the given Device+name
                else
                    delete all records the given Device
            else
                delete all records for all devices 
        */
        if (string.IsNullOrEmpty(DeviceName)) {
            // Delete property for all devices
            if (DeleteAllProperties) {
                foreach (var device in connection.Devices) {
                    device.Value.Properties.Clear();
                }
            } else {
                foreach (var device in connection.Devices) {
                    device.Value.Properties.Delete(this.PropertyName);
                }
            }
        } else {
            var device = connection.GetDeviceByName(DeviceName);
            if (device != null) {
                // Delete property on specific device
                if (DeleteAllProperties) {
                    device.Properties.Clear();
                } else {
                    device.Properties.Delete(this.PropertyName);
                }
            }
        }
    }

    public override string EncodeXml() {
        var el = new XElement("delProperty");
        if (!string.IsNullOrEmpty(DeviceName)) 
            el.Add(new XAttribute("device", DeviceName));
        if (!string.IsNullOrEmpty(PropertyName)) 
            el.Add(new XAttribute("name", PropertyName));
        if (!string.IsNullOrEmpty(Timestamp)) 
            el.Add(new XAttribute("timestamp", Timestamp));
        if (!string.IsNullOrEmpty(Message)) 
            el.Add(new XAttribute("message", Message));
        return el.ToString();
    }
}

public class IndiNotificationMessage : IndiDeviceMessage {
    public string DeviceName;
    public string Timestamp;
    public string Message;
    public IndiNotificationMessage(string device, string timestamp, string message) {
        this.DeviceName = device;
        this.Timestamp = timestamp;
        this.Message = message;
    }
    public override void Process(IndiConnection connection) {}

    public override string EncodeXml() {
        var el = new XElement("message");
        if (!string.IsNullOrEmpty(this.DeviceName))
            el.Add(new XAttribute("device", this.DeviceName));
        if (!string.IsNullOrEmpty(this.Timestamp))
            el.Add(new XAttribute("timestamp", this.Timestamp));
        if (!string.IsNullOrEmpty(this.Message))
            el.Add(new XAttribute("message", this.Message));
        return el.ToString();
    }
}

}