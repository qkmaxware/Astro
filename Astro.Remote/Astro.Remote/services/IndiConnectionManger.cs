using System;
using System.IO;
using System.Collections.Generic;
using Qkmaxware.Astro.Control;

namespace Astro.Remote.Services {

public class IndiLogger : IIndiListener {
    private string filename;
    private object key = new object();
    private TextWriter writer;

    public IndiLogger (string filename) {
        this.filename = filename;
        writer = AppDirectories.CreateFile(this.filename);
    }

    private void write(string message) {
        lock (this.key) {
            writer.WriteLine(message);
            writer.Flush();
        }
    }
    
    public void OnConnect(IndiServer server) {
        write($"CONNECTED {server.Host}:{server.Port}");
    }
    public void OnDisconnect(IndiServer server) {
        write($"DISCONNECTED {server.Host}:{server.Port}");
    }
    public void OnMessageSent(IndiClientMessage message) {
        write($"SENT {message.GetType()} {message.EncodeXml()}");
    }
    public void OnMessageReceived(IndiDeviceMessage message) {
        write($"RECEIVED {message.GetType()} {message.EncodeXml()}");
    }
    public void OnAddDevice(IndiDevice device) {
        write($"DEVICE CREATED {device.Name}");
    }
    public void OnRemoveDevice(IndiDevice device) {
        write($"DEVICE DELETED {device.Name}");
    }
}

public class IndiConnectionManager {

    private IndiServer server;
    private IndiConnection conn;

    private IndiLogger logger = new IndiLogger("indi.log");

    public bool IsConnected() {
        return conn != null && conn.IsConnected;
    }

    public void Subscribe(IIndiListener listener) {
        if (conn != null && listener != null) {
            conn.Subscribe(listener);
        }
    }

    public void Disconnect() {
        conn?.UnsubscribeAll(); // Clear all old subscribers
        conn?.Disconnect();     // Disconnect old connection
        conn = null;
    }

    public void RefreshAll() {
        if (conn != null) {
            conn.QueryProperties();
        }
    }

    public bool Connect(string host, int port, Action beforeRefresh = null, Action afterRefresh = null) {
        Disconnect();
        server = new IndiServer(host, port);
        if (server.TryConnect(out conn, logger)) {
            if (beforeRefresh != null) {
                beforeRefresh();
            }
            conn.QueryProperties();
            if (afterRefresh != null) {
                afterRefresh();
            }
            return true;
        } else {
            return false;
        }
    }

    public IEnumerable<IndiDevice> Devices() {
        if (conn == null || !conn.IsConnected)
            yield break;
        
        foreach (var device in conn.Devices) {
            yield return device.Value;
        }
    } 
}

}