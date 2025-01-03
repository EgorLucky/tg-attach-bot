using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using FileType = DomainLogic.Entities.FileType;

namespace DomainLogic
{
    public class TelegramDomainService
    {
        private readonly AppDbContext _appDb;
        private readonly ITelegramBotClient _tgClient;

        public TelegramDomainService(AppDbContext appDb, ITelegramBotClient tgClient)
        {
            _appDb = appDb;
            _tgClient = tgClient;
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

        public async Task<UserFile> SaveFile(Message? message)
        {
            var userFile = new UserFile()
            {
                Id = Guid.NewGuid(),
                TelegramUserId = message.From.Id,
                Name = "Unnamed File",
                CreatedAt = DateTimeOffset.UtcNow,
            };

            var fileUniqueId = GetUniqueFileIdFromMessage(message);
            if (!await _appDb.FileMetadata.AnyAsync(f => f.FileUniqueId == fileUniqueId))
            {
                try
                {
                    var file = CreateFileFromMessage(message);

                    await _appDb.AddAsync(file);
                    await _appDb.SaveChangesAsync();
                }
                catch(DbUpdateException ex)
                {
                    if (ex.InnerException?.Message?.Contains("23505") is true)
                    {
                        //unique key duplicate exception handling
                        var dbContextFileEntry = _appDb.ChangeTracker
                            .Entries<FileMetadata>()
                            .First(f => f.Entity.FileUniqueId == fileUniqueId);
                        dbContextFileEntry.State = EntityState.Detached;
                    }
                }
            }
            
            userFile.FileUniqueId = fileUniqueId;

            await _appDb.AddAsync(userFile);
            await _appDb.SaveChangesAsync();

            return userFile;
        }

        string GetUniqueFileIdFromMessage(Message message)
        {
            var fileUniqueId = "";
            switch (message.Type)
            {
                case MessageType.Photo:
                    fileUniqueId = message.Photo.Last().FileUniqueId;
                    break;
                case MessageType.Document:
                    fileUniqueId = message.Document.FileUniqueId;
                    break;
                case MessageType.Animation:
                    fileUniqueId = message.Animation.FileUniqueId;
                    break;
                case MessageType.Audio:
                    fileUniqueId = message.Audio.FileUniqueId;
                    break;
                case MessageType.Video:
                    fileUniqueId = message.Video.FileUniqueId;
                    break;
            }

            return fileUniqueId;
        }

        FileMetadata CreateFileFromMessage(Message? message)
        {
            var file = default(FileMetadata);
            switch (message.Type)
            {
                case MessageType.Photo:
                    file = CreateFile(message.Photo);
                    break;
                case MessageType.Document:
                    file = CreateFile(message.Document);
                    break;
                case MessageType.Animation:
                    file = CreateFile(message.Animation);
                    break;
                case MessageType.Audio:
                    file = CreateFile(message.Audio);
                    break;
                case MessageType.Video:
                    file = CreateFile(message.Video);
                    break;
            }

            return file;
        }

        FileMetadata? CreateFile(PhotoSize[]? photo)
        {
            var photoLast = photo.Last();
            var file = new FileMetadata()
            {
                FileId = photoLast.FileId,
                FileUniqueId = photoLast.FileUniqueId,
                Size = photoLast.FileSize.GetValueOrDefault(),
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

        FileMetadata? CreateFile(Document? document)
        {
            var fileType = FileType.Other;

            if (document.MimeType.StartsWith("image/")) fileType = FileType.Image;
            if (document.MimeType.StartsWith("audio/")) fileType = FileType.Audio;
            if (document.MimeType.StartsWith("video/")) fileType = FileType.Video;
            
            var file = new FileMetadata()
            {
                FileId = document.FileId,
                FileUniqueId = document.FileUniqueId,
                Size = document.FileSize.GetValueOrDefault(),
                MimeType = document.MimeType,
                ThumbFileId = document.Thumbnail?.FileId,
                FileType = fileType
            };

            return file;
        }

        FileMetadata? CreateFile(Animation? animation)
        {
            var file = new FileMetadata()
            {
                FileId = animation.FileId,
                FileUniqueId = animation.FileUniqueId,
                Size = animation.FileSize.GetValueOrDefault(),
                MimeType = animation.MimeType,
                Width = animation.Width,
                Height = animation.Height,
                ThumbFileId = animation.Thumbnail?.FileId,
                FileType = FileType.Animation
            };

            return file;
        }

        FileMetadata? CreateFile(Audio? audio)
        {
            var file = new FileMetadata()
            {
                FileId = audio.FileId,
                FileUniqueId = audio.FileUniqueId,
                Size = audio.FileSize.GetValueOrDefault(),
                MimeType = audio.MimeType,
                ThumbFileId = audio.Thumbnail?.FileId,
                Duration = audio.Duration,
                FileType = FileType.Audio
            };

            return file;
        }

        FileMetadata? CreateFile(Video? video)
        {
            var file = new FileMetadata()
            {
                FileId = video.FileId,
                FileUniqueId = video.FileUniqueId,
                Size = video.FileSize.GetValueOrDefault(),
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
