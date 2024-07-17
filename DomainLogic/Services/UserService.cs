using DomainLogic.DTOs.Output;
using Microsoft.EntityFrameworkCore;

namespace DomainLogic.Services;

public class UserService
{
    public AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetUserByIdResultDTO> GetUser(long telegramUserId)
    {
        var user = await _dbContext.TelegramUsers
            .FirstOrDefaultAsync(f => f.Id == telegramUserId);

        return new GetUserByIdResultDTO()
        {
            Success = user is not null,
            Result = user,
            ErrorMessage = user is null ? "not found" : ""
        };
    }
}