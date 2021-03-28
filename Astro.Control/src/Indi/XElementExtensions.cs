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

public static class XElementExtensions {
    public static void AddOrUpdateAttribute(this XElement element, string attribute, string value) {
        var attr = element.Attribute(attribute);
        if (attr == null) {
            element.Add(new XAttribute(attribute, value ?? string.Empty));
        } else {
            attr.Value = value;
        }
    }
}

}