namespace Qkmaxware.Astro.Control {
    
public interface IIndiListener {
    void OnConnect(IndiServer server);
    void OnDisconnect(IndiServer server);
    void OnMessageSent(IndiClientMessage message);
    void OnMessageReceived(IndiDeviceMessage message);
    void OnAddDevice(IndiDevice device);
    void OnRemoveDevice(IndiDevice device);
}

public class BaseIndiListener : IIndiListener {
    public virtual void OnConnect(IndiServer server) {}
    public virtual void OnDisconnect(IndiServer server) {}

    public virtual void OnMessageSent(IndiClientMessage message) {}
    public virtual void OnMessageReceived(IndiDeviceMessage message) {
        switch (message) {
            case IndiSetPropertyMessage smsg:
                OnSetProperty(smsg); break;
            case IndiDefinePropertyMessage dmsg:
                OnDefineProperty(dmsg); break;
            case IndiDeletePropertyMessage delmsg:
                OnDeleteProperty(delmsg); break;
            case IndiNotificationMessage note:
                OnNotification(note); break;
        }   
    }

    public virtual void OnAddDevice(IndiDevice device){}

    public virtual void OnDefineProperty(IndiDefinePropertyMessage message){}

    public virtual void OnDeleteProperty(IndiDeletePropertyMessage message){}

    public virtual void OnNotification(IndiNotificationMessage message){}

    public virtual void OnRemoveDevice(IndiDevice device){}

    public virtual void OnSetProperty(IndiSetPropertyMessage message){}
}

}