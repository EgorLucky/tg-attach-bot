using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = DomainLogic.Entities.File;
using FileType = DomainLogic.Entities.FileType;

namespace DomainLogic
{
    public class TelegramDomainService
    {
        private readonly AppDbContext _appDb;

        public TelegramDomainService(AppDbContext appDb, IHttpClientFactory factory)
        {
            _appDb = appDb;
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
                FileId = photoLast.FileId,
                Size = photoLast.FileSize.GetValueOrDefault(),
                Name = "Unnamed File",
                CreatedAt = DateTimeOffset.UtcNow,
                Width = photoLast.Width,
                Height = photoLast.Height,
                OtherPhotoSizes = photo.Where(p => p != photoLast).ToList(),
                FileType = FileType.Image
            };

            var thumb = photo.FirstOrDefault(p => p.Height == 320) 
                        ?? photo.FirstOrDefault();

            file.ThumbFileId = thumb.FileId;
            
            return file;
        }

        public async Task<File?> CreateFile(Document? document, long telegramUserId)
        {
            var fileType = document.MimeType switch
            {
                "image/jpeg" => FileType.Image,
                _ => FileType.Other
            };
            
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = document.FileId,
                Size = document.FileSize.GetValueOrDefault(),
                Name = document.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = document.MimeType,
                ThumbFileId = document.Thumbnail?.FileId,
                FileType = fileType
            };

            return file;
        }

        public async Task<File?> CreateFile(Animation? animation, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = animation.FileId,
                Size = animation.FileSize.GetValueOrDefault(),
                Name = animation.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = animation.MimeType,
                Width = animation.Width,
                Height = animation.Height,
                ThumbFileId = animation.Thumbnail?.FileId,
                FileType = FileType.Animation
            };

            return file;
        }

        public async Task<File?> CreateFile(Audio? audio, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = audio.FileId,
                Size = audio.FileSize.GetValueOrDefault(),
                Name = audio.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = audio.MimeType,
                ThumbFileId = audio.Thumbnail?.FileId,
                Duration = audio.Duration,
                FileType = FileType.Audio
            };

            return file;
        }

        public async Task<File?> CreateFile(Video? video, long telegramUserId)
        {
            var file = new File()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FileId = video.FileId,
                Size = video.FileSize.GetValueOrDefault(),
                Name = video.FileName,
                CreatedAt = DateTimeOffset.UtcNow,
                MimeType = video.MimeType,
                ThumbFileId = video.Thumbnail?.FileId,
                Duration = video.Duration,
                Width = video.Width,
                Height = video.Height,
                FileType = FileType.Video
            };

            return file;
        }

    }
}
