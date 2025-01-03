namespace DomainLogic.Entities;

public class UserFile
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public long TelegramUserId { get; set; }
    public string FileUniqueId { get; set; }
    public FileMetadata Content { get; set; }
    public string[]? KeyWords { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastUpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}