using Telegram.Bot.CommandRouting;
using Telegram.Bot.Types;

namespace DomainLogic
{
    public class AuthorizationChatHandler : IAuthorizationChatHandler
    {
        private readonly TelegramController _telegramController;

        public AuthorizationChatHandler(IEnumerable<TelegramBotController> controllers)
        {
            _telegramController = controllers.FirstOrDefault(c => c is TelegramController) as TelegramController;
        }
        public async Task<bool> CheckAuthorization(Update update)
        {
            if (update.Type is Telegram.Bot.Types.Enums.UpdateType.InlineQuery or Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult)
                return true;

            var chatId = update?.Message?.From?.Id ?? 0;
            return chatId > 0 
                   &&  _telegramController is not null 
                   && await _telegramController.RegisterIfNotRegistred(update.Message.From);
        }
    }
}