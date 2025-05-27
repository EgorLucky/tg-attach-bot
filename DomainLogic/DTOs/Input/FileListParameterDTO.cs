namespace DomainLogic.DTOs.Input;

public record FileListParameterDTO
{
    public int? UserId { get; init; }
    
    public SortedSet<string>? KeyWords { get; init; }

    public int Take { get; init; }
    
    public DateTimeOffset? Offset { get; init; }
    
    public SortedSet<Guid>? OffsetExcludedFileIds { get; init; }
    
    public FileListSources Source { get; init; }
}

public enum FileListSources
{
    Tagged,
    NotTagged
}