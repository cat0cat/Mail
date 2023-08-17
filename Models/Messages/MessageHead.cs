namespace YOApi.Models.Messages;

public class MessageHead
{
    public int Id { get; set; }
#nullable disable
    public string Title { get; set; }
    public string Sender { get; set; }
    public string Recipient { get; set; } 
#nullable enable
    public bool IsRead { get; set; }
#nullable disable
    public string Date { get; set; }
#nullable enable
    public List<string>? CopyTo { get; set; }

    public MessageHead()
    {
    }

    public MessageHead(Message message, List<string>? copyTo)
    {
        Id = message.Id;
        Title = message.Title;
        Sender = message.UserMessages.Single(x => x.Role == Role.Sender).User.Address;
        Recipient = message.UserMessages.Single(x => x.Role == Role.Recipient).User.Address;
        Date = message.Date.ToShortDateString();
        CopyTo = copyTo;
    }
}