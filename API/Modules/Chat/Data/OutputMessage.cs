namespace API.Modules.Chat.Data;
public class OutputMessage
{
    public string Username { get; set; }
    public string Time { get; set; }
    public string Message { get; set; }

    public OutputMessage(string username, string time, string message)
    {
        Username = username;
        Time = time;
        Message = message;
    }
}
