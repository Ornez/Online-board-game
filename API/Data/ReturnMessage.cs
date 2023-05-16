namespace API.Data;
public class ReturnMessage
{
    public string MessageType { get; set; }
    public bool Success { get; set; }

    public ReturnMessage()
    {
    }

    public ReturnMessage(string messageType, bool success)
    {
        MessageType = messageType;
        Success = success;
    }
}
 