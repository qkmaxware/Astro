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
using System.Collections;

namespace Qkmaxware.Astro.Control {

public class IndiPropertiesContainer : IEnumerable<KeyValuePair<string, IndiValue>>{
    private ConcurrentDictionary<string, IndiValue> properties = new ConcurrentDictionary<string, IndiValue>();

    public IndiPropertiesContainer() {}

    public bool HasProperty(string name) {
        return properties.ContainsKey(name);
    }

    public void Clear() {
        this.properties.Clear();
    }

    public void Delete(string property) {
        IndiValue old;
        this.properties.TryRemove(property, out old);
    }

    public IEnumerator<KeyValuePair<string, IndiValue>> GetEnumerator() {
        return this.properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    public IndiValue this[string key] {
        get => properties[key];
        internal set => properties[key] = value;
    }
}

public class IndiDevice {
    private IndiConnection conn;
    public string Name {get; private set;}
    public IndiPropertiesContainer Properties {get; private set;} = new IndiPropertiesContainer();

    public IndiDevice(string name, IndiConnection connection) {
        this.Name = name;
        this.conn = connection;
    }

    /// <summary>
    /// Send a request to update a partiular device property
    /// </summary>
    /// <param name="property">property name</param>
    public void RefreshProperty(string property) {
        // Send a request to get the properties for this device
        this.conn.Send(new IndiGetPropertiesMessage(this.Name, property));
    }

    /// <summary>
    /// Send a request to update all device properties
    /// </summary>
    public void RefreshProperties() {
        // Send a request to get the properties for this device
        this.conn.Send(new IndiGetPropertiesMessage(this.Name));
    }

    /// <summary>
    /// Send a request to update or modify a particular property
    /// </summary>
    /// <param name="name">property name</param>
    /// <param name="value">new property value</param>
    public void UpdateProperty(string name, IndiValue value) {
        // Create NewProperty client message
        // Populate device name, property name, and value
        var message = new IndiNewPropertyMessage(this.Name, name, value);
        // Update local copy
        // this.Properties[name] = value; 
        // Encode and send
        conn.Send(message);
        //RefreshProperty(name);
    }
}   

}