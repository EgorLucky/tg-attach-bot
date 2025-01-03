namespace DomainLogic.DTOs.Output;

public record DownloadedTelegramFileMetaDataDTO
{
    public string? ContentRange { get; init; }
    public string? ContentType { get; init; }
    public int StatusCode { get; init; }
}