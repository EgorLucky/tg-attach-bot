using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.CommandRouting;
using Telegram.Bot.CommandRouting.Attributes;
using Telegram.Bot.CommanndRouting;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = DomainLogic.Entities.File;

namespace DomainLogic
{
    [TelegramBotDoNotHandleMessagesFromMe]
    [TelegramBotDoNotHandlePrivateChat]
    [TelegramBotChatAuthorized]
    public class TelegramController : TelegramBotController
    {
        private readonly TelegramDomainService _telegramDomainService;
        private readonly ITelegramBotClient _bot;
        private BotConfiguration _botConfiguration;

        public TelegramController(
            TelegramDomainService telegramDomainService, 
            ITelegramBotClient bot, 
            BotConfiguration botConfiguration)
        {
            _telegramDomainService = telegramDomainService;
            _bot = bot;
            _botConfiguration = botConfiguration;
        }

        public async Task<bool> RegisterIfNotRegistred(User telegramUser)
        {
            return await _telegramDomainService.RegisterIfNotRegistred(telegramUser);
        }
        
        [TelegramBotAllowAnonymousChat]
        [TelegramBotTextCommand("/start")]
        public async Task StartChatWithUser(
            [FromUpdate(
                nameof(Update.Message),
                nameof(Message.From))] User telegramUser)
        {
            await  _telegramDomainService.RegisterIfNotRegistred(telegramUser);
            _bot.SendTextMessageAsync(telegramUser.Id,
                "Welcome! You can send messages with attachments here and i will save them for your farther access to them.");
        }
        
        [TelegramBotPhotoCommand]
        [TelegramBotDocumentCommand]
        [TelegramBotAnimationCommand]
        [TelegramBotAudioCommand]
        [TelegramBotVideoCommand]
        public async Task SendAttachment(
            [FromUpdate(
                nameof(Update.Message),
                nameof(Message.From))] User telegramUser)
        {
            var file = await _telegramDomainService.SaveFile(TelegramUpdate.Message);
            
            _bot.SendTextMessageAsync(telegramUser.Id,
                $"Your file successfully added! Click button below to add search tags and other information",
                replyToMessageId: TelegramUpdate.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Add info",$"https://t.me/{_botConfiguration.BotUsername}/attachment_form?startapp={file.Id}")
                    }
                }));
        }

    }
}