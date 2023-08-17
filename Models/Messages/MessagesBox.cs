namespace YOApi.Models.Messages;

public class MessagesBox
{
    public int Count { get; set; }
#nullable disable
    public MessageHead[] Messages { get; set; }
#nullable enable
}