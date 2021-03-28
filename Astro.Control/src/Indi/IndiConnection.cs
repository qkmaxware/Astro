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
using System.Text.RegularExpressions;

namespace Qkmaxware.Astro.Control {

public class IndiServer {
    public static readonly int DefaultPort = 7624;
    public string Host {get; private set;}
    public int Port {get; private set;}

    public IndiServer(string host, int port = 7624) {
        this.Host = host;
        this.Port = port;
    }

    public bool TryConnect(out IndiConnection conn) {
        return TryConnect(out conn, new IIndiListener[0]);
    }

    public bool TryConnect(out IndiConnection conn, params IIndiListener[] listeners) {
        conn = new IndiConnection(this);
        foreach (var listener in listeners) {
            conn.Subscribe(listener);
        }
        conn.ReConnect();
        if (conn.IsConnected) {
            return true;
        } else {
            conn.Disconnect();
            conn = null;
            return false;
        }
    }
}

public class IndiConnection : Notifier<IIndiListener> {
    private IndiServer server;
    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;

    public bool IsConnected => client != null && client.Connected;

    public ConcurrentDictionary<string, IndiDevice> Devices {get; private set;} = new ConcurrentDictionary<string, IndiDevice>();

    internal IndiConnection (IndiServer server) {
        var builder = new UriBuilder();
        this.server = server;
    }

    ~IndiConnection() {
        this.Disconnect();
    }

    public void AddDevice(IndiDevice device) {
        this.Devices.AddOrUpdate(device.Name, device, (key, old) => device);
        foreach (var sub in this.Subscribers) {
            sub.OnAddDevice(device);
        }
    }

    public void RemoveDevice(IndiDevice device) {
        IndiDevice deleted;
        this.Devices.TryRemove(device.Name, out deleted);
        foreach (var sub in this.Subscribers) {
            sub.OnRemoveDevice(deleted);
        }
    }

    public void RemoveDeviceByName(string name) {
        RemoveDevice(GetDeviceByName(name));
    }

    public IndiDevice GetDeviceByName(string name) {
        IndiDevice device;
        if (Devices.TryGetValue(name, out device)) {
            return device;
        } else {
            return null;
        }
    }

    public IndiDevice GetOrCreateDevice(string name) {
        int b4 = Devices.Count;
        var device = Devices.GetOrAdd(name, new IndiDevice(name, this));
        if (Devices.Count > b4) {
            foreach (var sub in this.Subscribers) {
                sub.OnAddDevice(device);
            }
        }
        return device;
    }

    public void QueryProperties() {
        this.Send(new IndiGetPropertiesMessage());
    }

    public void ReConnect() {
        if (!IsConnected) {
            try {
                client = new TcpClient(server.Host, server.Port);
                if (IsConnected) {
                    NetworkStream stream = client.GetStream();
                    reader = new StreamReader(stream, Encoding.UTF8);
                    writer = new StreamWriter(stream, Encoding.UTF8);

                    Task.Run(asyncRead);

                    foreach (var sub in this.Subscribers) {
                        sub.OnConnect(this.server);
                    }
                }
            } catch {
                Disconnect();
            }
        }
    }

    public void Disconnect() {
        client?.Close();
        client = null;
        reader = null;
        writer = null;
        foreach (var sub in this.Subscribers) {
            sub.OnDisconnect(this.server);
        }
    }

    public void Send(IndiClientMessage message) {
        this.sendXml(message.EncodeXml());
        foreach (var sub in this.Subscribers) {
            sub.OnMessageSent(message);
        }
    }

    public void Receive(IndiDeviceMessage message) {
        // actually do the correct action based on the message
        // adding devices or updating properties etc
        // allow blockers to continue
        if (message != null) {
            message.Process(this);
            foreach (var sub in this.Subscribers) {
                sub.OnMessageReceived(message);
            }
        }
    }

    public void sendXml(string xml) {
        if (IsConnected) {
            writer.Write(xml);
            writer.Flush();
        }
    }

    private void asyncRead() {
        StringBuilder str = new StringBuilder(client.Available);
        while(IsConnected) {
            try {
                while (reader != null && ((NetworkStream)reader.BaseStream).DataAvailable) {
                    char[] buffer = new char[client.Available];
                    reader.ReadBlock(buffer, 0, buffer.Length);
                    foreach (var c in buffer) {
                        if (XmlConvert.IsXmlChar(c)) {
                            str.Append(c);
                        }
                    }

                    if (tryParseXml(str.ToString())) {
                        str.Clear();
                    }
                }
            } catch {
                continue;
            }
        }
    }

    private bool tryParseXml(string xmllike) {
        // Parse XML 
        XmlDocument xmlDocument = new XmlDocument();
        try {  
            xmlDocument.LoadXml("<document>" + xmllike + "</document>");
        } catch {
            return false;
        }

        // Translate XML
        foreach (var child in xmlDocument.DocumentElement.ChildNodes) {
           
            if (child is XmlElement element) {
                if (element.Name.StartsWith("set")) {
                    var value = parseIndiValue(element);
                    Receive(
                        new IndiSetPropertyMessage(
                            element.GetAttribute("device"),
                            element.GetAttribute("name"),
                            value
                        )
                    );
                }
                else if (element.Name.StartsWith("def")) {
                    var value = parseIndiValue(element);
                    Receive(
                        new IndiDefinePropertyMessage(
                            element.GetAttribute("device"),
                            element.GetAttribute("name"),
                            value
                        )
                    );
                } 
                else if (element.Name == ("delProperty")) {
                    Receive(
                        new IndiDeletePropertyMessage(
                            element.GetAttribute("device"),
                            element.GetAttribute("name"),
                            element.GetAttribute("timestamp"),
                            element.GetAttribute("message")
                        )
                    );
                }
                else if (element.Name == ("message")) {
                    Receive(
                        new IndiNotificationMessage(
                            element.GetAttribute("device"),
                            element.GetAttribute("timestamp"),
                            element.GetAttribute("message")
                        )
                    );
                } 
                // Fallback
                else {}
            }
            
        }

        return true;
    }

    private IndiValue parseIndiValueVector<T>(string label, XmlElement value) where T:IndiValue {
        var vector = new IndiVector<T> {
            Name = value.GetAttribute("name"),
            Label = label,
            Group = value.GetAttribute("group"),
            State = value.GetAttribute("state"),
            Permissions = value.GetAttribute("perm"),
            Rule = value.GetAttribute("rule"),
            Timeout = value.GetAttribute("timeout"),
            Timestamp = value.GetAttribute("timestamp"),
            Comment = value.GetAttribute("message")
        };
        foreach (var child in value.ChildNodes) {
            if (child is XmlElement element) {
                var member = parseIndiValue(element);
                if (member is T valid)
                    vector.Add(valid);
            }
        }
        return vector;
    }
    private IndiValue parseIndiValue(XmlElement value) {
        var label = value.GetAttribute("label");
        if (string.IsNullOrEmpty(label)) {
            label = value.GetAttribute("name");
        }
        var name = value.GetAttribute("name");

        if (value.Name.EndsWith("TextVector")) {
            return parseIndiValueVector<IndiTextValue>(label, value);
        } 
        else if (value.Name.EndsWith("Text")) {
            return new IndiTextValue {
                Name = name,
                Label = label,
                Value = value.InnerText
            };
        } 
        else if (value.Name.EndsWith("NumberVector")) {
            return parseIndiValueVector<IndiNumberValue>(label, value);
        } 
        else if (value.Name.EndsWith("Number")) {
            var result = parseIndiDouble(value.InnerText, value.GetAttribute("format"));
            return new IndiNumberValue {
                Name = name,
                Label = label,
                Value = result,
                Min = double.Parse(value.GetAttribute("min")),
                Max = double.Parse(value.GetAttribute("max")),
                Step = double.Parse(value.GetAttribute("step"))
            };
        }
        else if (value.Name.EndsWith("SwitchVector")) {
            return parseIndiValueVector<IndiSwitchValue>(label, value);
        } 
        else if (value.Name.EndsWith("Switch")) {
            return new IndiSwitchValue {
                Name = name,
                Label = label,
                Switch = name,
                IsOn = value.InnerText == "On"
            };
        }
        // TODO handle lights
        else if (value.Name.EndsWith("LightVector")) {
            return parseIndiValueVector<IndiLightValue>(label, value);
        } 
        else if (value.Name.EndsWith("Light")) {
            return new IndiLightValue{
                Name = name,
                Label = label,
            };
        }
        // TODO handle blobs
        else if (value.Name.EndsWith("BLOBVector")) {
            return parseIndiValueVector<IndiBlobValue>(label, value);
        } 
        else if (value.Name.EndsWith("BLOB")) {
            return new IndiBlobValue {
                Name = name,
                Label = label,
                BlobString = value.InnerText
            };
        }

        // Default fallthrough case
        else {
            return null;
        }
    }

    private static Regex numberFormat = new Regex(@"(?<d>[+-]?\d+(?:\.\d+)?(?:[Ee][+-]?\d+)?)(?:\:(?<m>\d+(?:\.\d+)))?(?:\:(?<s>\d+(?:\.\d+)))?");
    private double parseIndiDouble(string str, string format) {
        /*
            Can be printf formats 
                https://www.cplusplus.com/reference/cstdio/printf/
            Or custom formats
                %<w>.<f>m
                <w> is the total field width
                <f> is the width of the fraction. valid values are:
                    9 -> :mm:ss.ss
                    8 -> :mm:ss.s
                    6 -> :mm:ss
                    5 -> :mm.m
                    3 -> :mm
        */
        // Hex based formats
        if (format == "x" || format == "X") {
            return int.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }
        // Decimal based formats 
        var match = numberFormat.Match(str);
        double degrees = double.Parse(match.Groups["d"].Value, System.Globalization.NumberStyles.Float);
        if(match.Groups["m"].Success) {
            degrees += double.Parse(match.Groups["m"].Value) * 60;
        }
        if (match.Groups["s"].Success) {
            degrees += double.Parse(match.Groups["s"].Value) * 3600;
        }
        return degrees;
    }
}

}