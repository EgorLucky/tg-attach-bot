using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = DomainLogic.Entities.File;

namespace DomainLogic
{
    public class TelegramDomainService
    {
        private readonly AppDbContext _appDb;
        private readonly HttpClient _httpClient;

        public TelegramDomainService(AppDbContext appDb, IHttpClientFactory factory)
        {
            _appDb = appDb;
            _httpClient = factory.CreateClient();
        }

        public async Task<bool> RegisterIfNotRegistred(User telegramUserDto)
        {
            if (!await _appDb.TelegramUsers.AnyAsync(c => c.Id == telegramUserDto.Id))
            {
                await Register(telegramUserDto);
            }

            return true;
        }

        public async Task Register(User telegramUserDto)
        {
            var telegramUser = new TelegramUser()
            {
                Id = telegramUserDto.Id,
                FirstName = telegramUserDto.FirstName,
                LastName = telegramUserDto.LastName,
                Username = telegramUserDto.Username,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _appDb.AddAsync(telegramUser);
            await _appDb.SaveChangesAsync();
        }
        
        public async Task<File> SaveFile(Message? message)
        {
            var file = default(File);
            switch (message.Type)
            {
                case MessageType.Photo:
                {
                    file = await CreateFile(message.Photo, message.From.Id);
                }
                    break;
                case MessageType.Document:
                {
                    file = await CreateFile(message.Document, message.From.Id);
                }
                    break;
                case MessageType.Animation:
                {
                    file = await CreateFile(message.Animation, message.From.Id);
                }
                    break;
                case MessageType.Audio:
                {
                    file = await CreateFile(message.Audio, message.From.Id);
                }
                    break;
                case MessageType.Video:
                {
                    file = await CreateFile(message.Video, message.From.Id);
                }
                    break;
            }
            await _appDb.AddAsync(file);
            await _appDb.SaveChangesAsync();

            return file;
        }

        public async Task<File?> CreateFile(PhotoSize[]? photo, long telegramUserId)
        {
            var photoLast = photo.Last();
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = photoLast.FileUniqueId,
                Size = photoLast.FileSize.GetValueOrDefault(),
                Name = "Unnamed File",
                CreatedAt = DateTimeOffset.UtcNow,
                Width = photoLast.Width,
                Height = photoLast.Height,
                OtherPhotoSizes = photo.Where(p => p != photoLast).ToList()
            };

            var thumb = photo.FirstOrDefault(p => p.Height == 320) 
                        ?? photo.FirstOrDefault();

            file.ThumbFileId = thumb.FileUniqueId;
            
            return file;
        }

        public async Task<File?> CreateFile(Document? document, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = document.FileUniqueId,
                Size = document.FileSize.GetValueOrDefault(),
                Name = document.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = document.MimeType,
                ThumbFileId = document.Thumbnail?.FileUniqueId
            };

            return file;
        }

        public async Task<File?> CreateFile(Animation? animation, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = animation.FileUniqueId,
                Size = animation.FileSize.GetValueOrDefault(),
                Name = animation.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = animation.MimeType,
                Width = animation.Width,
                Height = animation.Height,
                ThumbFileId = animation.Thumbnail?.FileUniqueId
            };

            return file;
        }

        public async Task<File?> CreateFile(Audio? audio, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = audio.FileUniqueId,
                Size = audio.FileSize.GetValueOrDefault(),
                Name = audio.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = audio.MimeType,
                ThumbFileId = audio.Thumbnail?.FileUniqueId,
                Duration = audio.Duration
            };

            return file;
        }

        public async Task<File?> CreateFile(Video? video, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = video.FileUniqueId,
                Size = video.FileSize.GetValueOrDefault(),
                Name = video.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = video.MimeType,
                ThumbFileId = video.Thumbnail?.FileUniqueId,
                Duration = video.Duration,
                Width = video.Width,
                Height = video.Height
            };

            return file;
        }

    }
}
