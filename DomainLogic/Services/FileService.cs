using DomainLogic.DTOs.Input;
using DomainLogic.DTOs.Output;
using Microsoft.EntityFrameworkCore;

namespace DomainLogic.Services;

public class FileService
{
    public AppDbContext _dbContext;

    public FileService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetFileByIdResultDTO> GetFileById(Guid id, long telegramUserId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == id 
                                      && f.TelegramUserId == telegramUserId 
                                      && f.DeletedAt == null);

        return new GetFileByIdResultDTO()
        {
            Success = file is not null,
            Result = file,
            ErrorMessage = file is null ? "not found" : "",
            ErrorType = file is null ? ErrorType.NotFound : null 
        };
    }

    public async Task<GetFileByIdResultDTO> Update(UpdateFileDTO dto, long telegramUserId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == dto.Id 
                                      && f.TelegramUserId == telegramUserId 
                                      && f.DeletedAt == null);
        
        if (file is null)
            return new GetFileByIdResultDTO() { ErrorMessage = "not found", ErrorType = ErrorType.NotFound };

        file.Name = dto.Name;
        file.KeyWords = dto.KeyWords.ToArray();
        file.LastUpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new GetFileByIdResultDTO() { Success = true, Result = file };
    }
    
    public async Task<GetFileByIdResultDTO> Delete(Guid id, long telegramUserId)
    {
        var file = await _dbContext.Files
            .FirstOrDefaultAsync(f => f.Id == id 
                                      && f.TelegramUserId == telegramUserId
                                      && f.DeletedAt == null);
        
        if (file is null)
            return new GetFileByIdResultDTO() { ErrorMessage = "not found", ErrorType = ErrorType.NotFound };

        file.DeletedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new GetFileByIdResultDTO() { Success = true, Result = file };
    }
}