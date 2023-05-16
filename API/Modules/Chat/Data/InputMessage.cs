namespace API.Modules.Chat.Data;
public class InputMessage
{
    public string Message { get; set; }

    public InputMessage(string message)
    {
        Message = message;
    }
}
