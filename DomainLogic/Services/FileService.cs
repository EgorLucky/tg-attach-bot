using System.Linq.Expressions;
using DomainLogic.DTOs.Input;
using DomainLogic.DTOs.Output;
using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis.Extensions.Core.Abstractions;
using File = DomainLogic.DTOs.Output.File;

namespace DomainLogic.Services;

public class FileService
{
    private readonly AppDbContext _dbContext;

    public FileService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetFileByIdResultDTO> GetFileById(Guid id, long telegramUserId)
    {
        var file = await _dbContext.UserFiles
            .Where(f => f.Id == id
                        && f.TelegramUserId == telegramUserId
                        && f.DeletedAt == null)
            .Select(MapEntityModelToDto)
            .FirstOrDefaultAsync();

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
        var file = await _dbContext.UserFiles
            .Include(uf => uf.Content)
            .FirstOrDefaultAsync(uf => uf.Id == dto.Id
                                      && uf.TelegramUserId == telegramUserId
                                      && uf.DeletedAt == null);

        if (file is null)
            return new GetFileByIdResultDTO() { ErrorMessage = "not found", ErrorType = ErrorType.NotFound };

        file.Name = dto.Name;
        file.KeyWords = dto.KeyWords.ToArray();
        file.LastUpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new GetFileByIdResultDTO() { Success = true, Result = MapEntityModelToDtoFunc(file) };
    }

    public async Task<GetFileByIdResultDTO> Delete(Guid id, long telegramUserId)
    {
        var file = await _dbContext.UserFiles
            .FirstOrDefaultAsync(f => f.Id == id
                                      && f.TelegramUserId == telegramUserId
                                      && f.DeletedAt == null);

        if (file is null)
            return new GetFileByIdResultDTO() { ErrorMessage = "not found", ErrorType = ErrorType.NotFound };

        file.DeletedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new GetFileByIdResultDTO() { Success = true, Result = new () { Id = id } };
    }

    public async Task<FileListResultDTO> GetList(FileListParameterDTO parameters, long userId)
    {
        var query = _dbContext.UserFiles.AsQueryable();

        if (parameters.UserId is not null)
            query = query.Where(f => f.TelegramUserId == userId);

        if (parameters.Source == FileListSources.Tagged)
        {
            query = query.Where(f => f.KeyWords != null);
            if (parameters.KeyWords is not null && parameters.KeyWords.Any())
                foreach (var keyWord in parameters.KeyWords)
                    query = query.Where(f => f.KeyWords.Contains(keyWord));
        }
        else query = query.Where(f => f.KeyWords == null);

        query = query.OrderByDescending(f => f.CreatedAt);

        if (parameters.Offset is not null)
        {
            query = query.Where(f => f.CreatedAt <= parameters.Offset);
            
            if (parameters.OffsetExcludedFileIds is not null)
                query = query.Where(f => !parameters.OffsetExcludedFileIds.Contains(f.Id));
        }

        query = query.Take(parameters.Take);

        var results = await query
            .Include(uf => uf.Content)
            .Select(MapEntityModelToDto)
            .ToListAsync();

        return new FileListResultDTO { Success = true, Result = results };
    }

    static Expression<Func<UserFile, File>> MapEntityModelToDto = (model) => new File
    {
        Id = model.Id,
        Name = model.Name,
        TelegramUserId = model.TelegramUserId,
        KeyWords = model.KeyWords,
        FileId = model.Content.FileId,
        Size = model.Content.Size,
        Width = model.Content.Width,
        Height = model.Content.Height,
        Duration = model.Content.Duration,
        MimeType = model.Content.MimeType,
        FileType = model.Content.FileType,
        CreatedAt = model.CreatedAt,
        LastUpdatedAt = model.LastUpdatedAt
    };

    static Func<UserFile, File>? _mapEntityModelToDtoCompiled;

    static Func<UserFile, File> MapEntityModelToDtoFunc 
    {
        get
        {
            if (_mapEntityModelToDtoCompiled is null)
                _mapEntityModelToDtoCompiled = MapEntityModelToDto.Compile();
            return _mapEntityModelToDtoCompiled;
        }
    }
}