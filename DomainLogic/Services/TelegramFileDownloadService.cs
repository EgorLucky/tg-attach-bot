using System.Net.Http.Headers;
using DomainLogic.DTOs.Output;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using Telegram.Bot;

namespace DomainLogic.Services;

public class TelegramFileDownloadService
{
    private readonly BotConfiguration _botConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRedisDatabase _redisDatabase;
    private readonly ITelegramBotClient _tgClient;
    private readonly ILogger<TelegramFileDownloadService> _logger;


    public TelegramFileDownloadService(
        BotConfiguration botConfiguration, 
        IHttpClientFactory httpClientFactory, 
        IRedisDatabase redisDatabase, 
        ITelegramBotClient tgClient, 
        ILogger<TelegramFileDownloadService> logger)
    {
        _botConfiguration = botConfiguration;
        _httpClientFactory = httpClientFactory;
        _redisDatabase = redisDatabase;
        _tgClient = tgClient;
        _logger = logger;
    }

    public async Task DownloadFile(
        string fileId,
        string? rangeHeader,
        Action<DownloadedTelegramFileMetaDataDTO> beforeCopyToStreamAction,
        Stream fileStream)
    {

        var filePath = await GetFilePath(fileId);
        
        var httpClient = _httpClientFactory.CreateClient();

        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new($"https://api.telegram.org/file/bot{_botConfiguration.BotToken}/{filePath}"),
        };
        
        if (rangeHeader is not null)
            httpRequest.Headers.Range = RangeHeaderValue.Parse(rangeHeader);

        var response = await httpClient.SendAsync(httpRequest);

        var result = new DownloadedTelegramFileMetaDataDTO
        {
            ContentRange = response.Content.Headers.ContentRange?.ToString(),
            ContentType = response.Content.Headers.ContentType?.ToString(),
            StatusCode = (int)response.StatusCode
        };
        beforeCopyToStreamAction?.Invoke(result);
        await response.Content.CopyToAsync(fileStream);
    }

    /// TODO: move to separated service
    async Task<string> GetFilePath(string fileId)
    {
        _logger.LogInformation($"Getting filePath from from redis by fileId={fileId}");
        var filePath = await _redisDatabase.GetAsync<string>(fileId);

        if (filePath is null)
        {
            _logger.LogInformation($"Got null filePath from from redis by fileId={fileId}");
            var lockRecordKey = $"add-lock-{fileId}";
            _logger.LogInformation($"Adding lock-record on getting filePath from telegram-api by fileId={fileId}");
            var lockAdded =  await _redisDatabase.AddAsync(lockRecordKey, filePath, TimeSpan.FromSeconds(5), When.NotExists);
            
            if (lockAdded)
            {
                _logger.LogInformation($"Added lock-record on getting filePath from telegram-api by fileId={fileId}");
                _logger.LogInformation($"Getting filePath from telegram-api by fileId={fileId}");
                filePath = (await _tgClient.GetFileAsync(fileId)).FilePath;
                _logger.LogInformation($"Got filePath from telegram-api by fileId={fileId}");
                _logger.LogInformation($"Adding filePath to redis with fileId={fileId}");
                var filePathAdded = await _redisDatabase.AddAsync(fileId, filePath, TimeSpan.FromHours(1), When.NotExists);
                var lockReleased = await _redisDatabase.RemoveAsync(lockRecordKey);
                if (!filePathAdded)
                {
                    //TODO: handle filePath not added 
                    _logger.LogInformation($"FilePath was not added to redis with fileId={fileId}");
                }

                if (!lockReleased)
                {
                    //TODO: handle lock not released
                    _logger.LogInformation($"Lock-record on getting filePath from telegram-api was not released with fileId={fileId}");
                }
            }
            else
            {
                _logger.LogInformation($"Lock-record on getting filePath from telegram-api was already added by fileId={fileId}");
                _logger.LogInformation($"Repeating get filePath from redis until result will be not null by fileId={fileId}");
                //awaiting filePath adding from another request
                var repeatBeginDateTime = DateTimeOffset.UtcNow;
                var maxTimeSpan = TimeSpan.FromSeconds(5);
                var count = 0;
                while (filePath is null && DateTimeOffset.UtcNow < repeatBeginDateTime + maxTimeSpan)
                {
                    filePath = await _redisDatabase.GetAsync<string>(fileId);
                    count++;
                    _logger.LogInformation($"Done retry #{count} of getting filePath from from redis by fileId={fileId}");
                }
                if (filePath is not null)
                    _logger.LogInformation($"Got filePath from redis after repeating queries after {(DateTimeOffset.UtcNow - repeatBeginDateTime).TotalMilliseconds} ms by fileId={fileId}");
            }
        }
        
        return filePath;
    }
    
}