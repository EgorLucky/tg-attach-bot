using System.ComponentModel.DataAnnotations;

namespace DomainLogic.DTOs.Input;

public record UpdateFileDTO
{
    [Required] 
    public Guid Id { get; init; }
    
    [Required]
    public string Name { get; init; }
    
    [Required]
    public SortedSet<string> KeyWords { get; init; }
}