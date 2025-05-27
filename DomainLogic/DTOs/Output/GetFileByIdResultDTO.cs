using DomainLogic.Entities;

namespace DomainLogic.DTOs.Output;

public record GetFileByIdResultDTO : BaseResultObjectDataDTO<File?>;

public record File
{
    public Guid Id { get; init; }
    public string? Name { get; init; } 
    public long TelegramUserId { get; init; }
    public string FileId { get; init; }
    public long Size { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public int? Duration { get; init; }
    public string? MimeType { get; init; }
    public FileType FileType { get; init; }
    public string[]? KeyWords { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}