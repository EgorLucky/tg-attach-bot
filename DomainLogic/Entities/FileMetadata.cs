using Telegram.Bot.Types;

namespace DomainLogic.Entities;

public class FileMetadata
{
    public string FileUniqueId { get; set; }
    public string FileId { get; set; }
    public long Size { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public string? ThumbFileId { get; set; }
    public string? MimeType { get; set; }
    public FileType FileType { get; set; }
    public List<PhotoSize> OtherPhotoSizes { get; set; }
}

public enum FileType
{
    Image,
    Animation,
    Video,
    Audio,
    Other
}