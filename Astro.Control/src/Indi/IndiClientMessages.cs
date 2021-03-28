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

public abstract class IndiClientMessage {
    public abstract string EncodeXml();
}

public class IndiGetPropertiesMessage : IndiClientMessage {
    public string DeviceName;
    public string PropertyName;
    public IndiGetPropertiesMessage () : this(null) {}
    public IndiGetPropertiesMessage (string device) {
        this.DeviceName = device;
        this.PropertyName = null;
    }
    public IndiGetPropertiesMessage (string device, string property) {
        this.DeviceName = device;
        this.PropertyName = property;
    }
    public override string EncodeXml() {
        var el = new XElement("getProperties",
                new XAttribute("version", "1.7"));
        if (!string.IsNullOrEmpty(DeviceName)) {
            el.Add(new XAttribute("device", this.DeviceName));
        }
        if (!string.IsNullOrEmpty(PropertyName)) {
            el.Add(new XAttribute("name", this.PropertyName));
        }
        return el.ToString();
    }
}

public class IndiNewPropertyMessage : IndiClientMessage {
    public string DeviceName;
    public string PropertyName;
    public IndiValue PropertyValue;
    public IndiNewPropertyMessage(string device, string prop, IndiValue value) {
        this.DeviceName = device;
        this.PropertyName = prop;
        this.PropertyValue = value;
    }
    public override string EncodeXml() {
        var el = PropertyValue.CreateNewElement();
        el.AddOrUpdateAttribute("device", this.DeviceName);
        el.AddOrUpdateAttribute("name", this.PropertyName);
        return el.ToString();
    }
}

}