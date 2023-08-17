using YOApi.Models.Messages;

namespace YOApi.Data.Interfaces;

public class RecepientNotFoundException : Exception
{
    public List<string> Value {get;}
    public RecepientNotFoundException(List<string> resipient): base() 
    { 
        Value = resipient;
    }
}

public interface IMessageRepository
{
    public Task CreateMessage(MessageFromClient message, int userId, CancellationToken token);
    public Task<MessageBody?> GetMessageById(int messageId, int userId, CancellationToken token);
    public Task<bool> MessageUpdateReadStatus(int messageId, CancellationToken token);
    public Task<bool> MessageUpdateArchiveStatus(List<int> messageId, int userId, CancellationToken token);
    public Task<MessagesBox> GetMessagesByUserId(int userId, MailBoxes selection, ArchiveBoxes location, CancellationToken token);
    public Task<MessagesBox> Search(MailBoxes mailSelection, ArchiveBoxes archiveSelection, int userId, string searchString, CancellationToken token); 
}
