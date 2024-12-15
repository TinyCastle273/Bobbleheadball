public class Message
{
    public MessageBusType messageType;
    //public GameObject sender;
    public object data;
    public Message() { }

    public Message(MessageBusType type)
    {
        messageType = type;
    }

    public Message(MessageBusType type, object data)
    {
        messageType = type;
        this.data = data;
    }
}