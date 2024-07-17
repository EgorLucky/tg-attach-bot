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
            .FirstOrDefaultAsync(f => f.Id == id && f.TelegramUserId == telegramUserId);

        return new GetFileByIdResultDTO()
        {
            Success = file is not null,
            Result = file,
            ErrorMessage = file is null ? "not found" : ""
        };
    }
}