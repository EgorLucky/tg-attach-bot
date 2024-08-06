using Telegram.Bot.Types;

namespace DomainLogic.Entities;

public class File
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public long TelegramUserId { get; set; }
    public string? MimeType { get; set; }
    public string[]? KeyWords { get; set; }
    public string FileId { get; set; }
    public string? FilePath { get; set; }
    public long Size { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public string? ThumbFileId { get; set; }
    public FileType FileType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastUpdatedAt { get; set; }
    public List<PhotoSize> OtherPhotoSizes { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public enum FileType
{
    Image,
    Animation,
    Video,
    Audio,
    Other
}