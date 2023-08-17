using System.ComponentModel.DataAnnotations;

namespace YOApi.Models.Messages;

public class Message
{
    public int Id { get; set; }

    [MaxLength(100)]
#nullable disable
    public string Title { get; set; }
#nullable enable

    [MaxLength(1000)]
    public string? Body { get; set; }
    public DateTime Date { get; set; }
#nullable disable
    public ICollection<User> Users { get; set; }
    public ICollection<UserMessage> UserMessages { get; set; }
#nullable enable

    public Message()
    {

    }

    public Message(MessageFromClient messageData, List<UserMessage> userMessages)
    {
        Title = messageData.Title;
        Body = messageData.Title;
        Date = DateTime.UtcNow;
        UserMessages = userMessages;
    }
}