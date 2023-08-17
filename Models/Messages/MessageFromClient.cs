namespace YOApi.Models.Messages;

public class MessageFromClient
{
#nullable disable
    public string Title { get; set; }
#nullable enable
    public string? Body { get; set; }
#nullable disable
    public string Recipient { get; set; }
    public List<string> CopyTo { get; set; }
#nullable enable
}
