using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections;

namespace Qkmaxware.Astro.Control {

public abstract class IndiValue {
    public string Name;
    public string Label;
    public abstract string IndiTypeName {get;}
    internal abstract XElement CreateElement(string prefix, string subPrefix);
    public XElement CreateNewElement() {
        return CreateElement("new", "one");
    }
    public XElement CreateSetElement() {
        return CreateElement("set", "one");
    }
    public XElement CreateDefinitionElement() {
        return CreateElement("def", "def");
    }
}

public abstract class UpdatableIndiValue : IndiValue {
    public abstract bool TryUpdateValue(IndiValue value);
}

public abstract class IndiValue<T> : UpdatableIndiValue {
    public virtual T Value { get; set; }

    public override bool TryUpdateValue(IndiValue from) {
        if (from != null && from is IndiValue<T> valueType) {
            this.UpdateValue(valueType);
            return true;
        } else {
            return false;
        }
    }
    public virtual void UpdateValue(IndiValue<T> from) {
        this.Value = from.Value;
    }
}

public class IndiTextValue : IndiValue<string> {
    public override string IndiTypeName => "Text";
    internal override XElement CreateElement(string prefix, string subPrefix) {
        var node = new XElement(
            prefix + IndiTypeName, 
            new XText(this.Value ?? string.Empty)
        );
        if (this.Name != null) {
            node.Add(new XAttribute("name", this.Name));
        }
        if (this.Label != null) {
            node.Add(new XAttribute("label", this.Label));
        }
        return node;
    }

    public override string ToString() {
        return Value.ToString();
    }
    
}

public class IndiNumberValue : IndiValue<double> {
    public double Min;
    public double Max;
    public double Step;
    public override string IndiTypeName => "Number";
    internal override XElement CreateElement(string prefix, string subPrefix) {
        var node = new XElement(
            prefix + IndiTypeName, 
            new XAttribute("format", "%f"),
            new XAttribute("min", this.Min),
            new XAttribute("max", this.Max),
            new XAttribute("step", this.Step),
            new XText(this.Value.ToString())
        );
        if (this.Name != null) {
            node.Add(new XAttribute("name", this.Name));
        }
        if (this.Label != null) {
            node.Add(new XAttribute("label", this.Label));
        }
        return node;
    }

    public override string ToString() {
        return Value.ToString();
    }
}

public class IndiSwitchValue : IndiValue<bool> {
    public string Switch;
    public bool IsOn => Value == true;
    public override string IndiTypeName => "Switch";
    internal override XElement CreateElement(string prefix, string subPrefix) {
        var node = new XElement(
            prefix + IndiTypeName, 
            new XText(this.IsOn ? "On": "Off")
        );
        if (this.Name != null) {
            node.Add(new XAttribute("name", this.Name));
        }
        if (this.Label != null) {
            node.Add(new XAttribute("label", this.Label));
        }
        return node;
    }

    public override string ToString() {
        return this.IsOn ? "On": "Off";
    }
}

public class IndiLightValue : IndiValue {
    public override string IndiTypeName => "Light";
    internal override XElement CreateElement(string prefix, string subPrefix) {
        throw new NotImplementedException();
    }
}

public class IndiBlobValue : IndiValue<string> {
    public byte[] Blob => System.Text.Encoding.ASCII.GetBytes(this.Value);
    public override string IndiTypeName => "BLOB";
    public IndiBlobValue() {}
    public IndiBlobValue(FileStream fs) {
        using (BinaryReader reader = new BinaryReader(fs)) {
            byte[] blob = reader.ReadBytes((int)fs.Length);
            this.Value = System.Text.Encoding.ASCII.GetString(blob);
        }
    }
    internal override XElement CreateElement(string prefix, string subPrefix) {
        var node = new XElement(
            prefix + IndiTypeName, 
            new XText(Value ?? string.Empty)
        );
        if (this.Name != null) {
            node.Add(new XAttribute("name", this.Name));
        }
        if (this.Label != null) {
            node.Add(new XAttribute("label", this.Label));
        }
        return node;
    }

    public override string ToString() {
        return "[BLOB]";
    }
}

public class IndiVector<T> : UpdatableIndiValue, IList<T> where T:IndiValue {

    public string Group;
    public string State;
    public string Permissions = "rw";
    public bool IsReadOnly => Permissions == "r";
    public bool IsReadWrite => Permissions == "rw";
    public bool IsWritable => Permissions.Contains("w");
    public string Rule;
    public string Timeout;
    public string Timestamp;
    public string Comment;

    private List<T> vector = new List<T>();

    public T this[int index] { 
        get => vector[index]; 
        set { if(IsWritable) { vector[index] = value; } }
    }

    public int Count => vector.Count;

    public IndiVector () {}
    public IndiVector (string name) {
        this.Name = name;
    }

    public T WithName(string name) {
        return this.vector.Where(value => value.Name == name).FirstOrDefault();
    }

    public void Add(T item) {
        vector.Add(item);
    }

    public void Clear() {
        vector.Clear();
    }

    public bool Contains(T item) {
        return vector.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        vector.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator() {
        return vector.GetEnumerator();
    }

    public int IndexOf(T item) {
        return vector.IndexOf(item);
    }

    public void Insert(int index, T item) {
        vector.Insert(index, item);
    }

    public bool Remove(T item) {
        return vector.Remove(item);
    }

    public void RemoveAt(int index) {
        vector.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return vector.GetEnumerator();
    }

    public override string IndiTypeName => this.vector[0].IndiTypeName + "Vector"; // Only works if there is at least 1 element
    internal override XElement CreateElement(string prefix, string subPrefix) {
        var parent = new XElement(
            prefix + IndiTypeName
        );
        if (this.Name != null)
            parent.Add(new XAttribute("name", this.Name));
        if (this.Label != null)
            parent.Add(new XAttribute("label", this.Label));
        if (this.Group != null)
            parent.Add(new XAttribute("group", this.Group));
        if (this.State != null)
            parent.Add(new XAttribute("state", this.State));
        if (this.Permissions != null)
            parent.Add(new XAttribute("perm", this.Permissions));
        if (this.Rule != null)
            parent.Add(new XAttribute("rule", this.Rule));

        foreach (var child in this.vector) {
            parent.Add(child.CreateElement(subPrefix, subPrefix));
        }
        return parent;
    }

    /// <summary>
    /// Try to update the values of this vector from another vector
    /// </summary>
    /// <param name="value">indi values to draw from</param>
    /// <returns>true if indi value is a compatible type vector</returns>
    public override bool TryUpdateValue(IndiValue value) {
        if (value != null && value is IndiVector<T> vec) {
            var updates = vec.vector;
            List<T> newVector = new List<T>(vector.Count);
            foreach (var update in updates) {
                // Access existing property
                var existingProperty = vector.Where(prop => prop.Name == update.Name).FirstOrDefault();
                if (existingProperty != null && existingProperty is UpdatableIndiValue updatableProperty) {
                    if (updatableProperty.TryUpdateValue(update)) {
                        // Updated existing property, add back to the list
                        newVector.Add(existingProperty);
                    } else {
                        // Failed to update existing property, add the new value as raw
                        newVector.Add(update);
                    }
                } else {
                    // No previous property, add new value as raw
                    newVector.Add(update);
                }
            }
            this.vector = newVector;
            return true;
        } else {
            return false;
        }
    }
}

/// <summary>
/// Extention methods for different vector types
/// </summary>
public static class IndiVectorExtentions {
    /// <summary>
    /// Enable a specific switch
    /// </summary>
    /// <param name="options">list of possible switch values</param>
    /// <param name="name">name of the option to enable</param>
    public static void SwitchTo(this IndiVector<IndiSwitchValue> options, string name) {
        foreach (var option in options) {
            option.Value = option.Name == name;
        }
    }
    /// <summary>
    /// Enable a specific switch
    /// </summary>
    /// <param name="options">list of possible switch values</param>
    /// <param name="option">index of the option to enable</param>
    public static void SwitchTo(this IndiVector<IndiSwitchValue> options, int option) {
        for(var i = 0; i < options.Count; i++) {
            options[i].Value = i == option;
        }
    }

    /// <summary>
    /// Enable a specific switch
    /// </summary>
    /// <param name="options">list of possible switch values</param>
    /// <param name="selector">function to select switch</param>
    public static void SwitchTo(this IndiVector<IndiSwitchValue> options, Func<IndiSwitchValue, bool> selector) {
        foreach (var toggle in options) {
            toggle.Value = selector(toggle);
        }
    }

    /// <summary>
    /// Get the first switch with the given name from the vector
    /// </summary>
    /// <param name="options">list of possible switch values</param>
    /// <param name="name">name of the switch</param>
    /// <returns>switch</returns>
    public static IndiSwitchValue GetSwitch (this IndiVector<IndiSwitchValue> options, string name) {
        return options.Where(opt => opt.Name == name).First();
    }
}

}