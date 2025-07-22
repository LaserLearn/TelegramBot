using Microsoft.AspNetCore.Components.Forms;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.contract.Bot.channel;
using TelegramBot.model.Enum;
using TelegramBot.Services.channel;
using TelegramBot.Tool;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
namespace TelegramBot.handlers.channel
{
    public class ChannelCommandHandler : IChannelHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly ChannelService _channelService;
        public ChannelCommandHandler(TelegramBotClient botClient, ChannelService channelService)
        {
            _botClient = botClient;
            _channelService = channelService;
        }
        public async Task<bool> ShouldHandleAsync(Update update)
        {
            long? chatId = UserStateTools.GetChatId(update);
             
            if (chatId == null)
                return false;
            
            var step = await UserStateTools.GetStateAsync(chatId.Value);
            


            if (update.CallbackQuery != null && update.CallbackQuery.Data == "like")
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId.Value,
                    text: "👍 کاربر این پیام را لایک کرد.");
            }
            else if (update.CallbackQuery != null && update.CallbackQuery.Data == "dislike")
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId.Value,
                    text: "👎 کاربر این پیام را دیسلایک کرد.");
            }

            if (update.CallbackQuery != null && update.CallbackQuery.Data == "Channel" ||
                await UserStateTools.GetMotherStateAsync(chatId) == nameof(UserBotState.InChannel))
            {
                await UserStateTools.SetMotherAsync(chatId: chatId, nameof(UserBotState.InChannel));
                return true;
            }
            return false;
        }

        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            long? chatId = UserStateTools.GetChatId(update);

            var step = await UserStateTools.GetStateAsync(chatId.Value);

            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case nameof(ChannelState.SendImage):
                        await EnterImageAsync(update , cancellationToken);
                        return;
                    
                    case nameof(ChannelState.SendMessage):
                        await EnterMessageAsync(update, cancellationToken);
                        return;
                    
                    case nameof(ChannelState.SendPoll):
                        await HandleSendChannelPollAsync(update, cancellationToken);
                        return;
                    default:
                        await _channelService.ShowMainMenuAsync(chatId ?? new long(), cancellationToken);
                        return;
                }
            }
            switch (step)
            {
                case nameof(ChannelState.WatingImage):
                    await HandleSendChannelImageAsync(update , cancellationToken);
                    break;
                case nameof(ChannelState.WatingMessage):
                    await HandleSendChannelMessageAsync(update, cancellationToken);
                    break;
                default:
                    return;
            }
        }

        public Task<bool> HandleSendChannelVideoAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EnterMessageAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            await _botClient.SendTextMessageAsync(
                chatId.Value,
                "لطفاً پیام را وارد کنید.",
                cancellationToken: cancellationToken
                );

            await UserStateTools.SetStateAsync(chatId, nameof(ChannelState.WatingMessage));

            return true;
        }
        public async Task<bool> HandleSendChannelMessageAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message?.Chat?.Id;
            var text = update.Message?.Text;

            if (chatId == null || string.IsNullOrEmpty(text))
                return false;

            string channelUsernameOrId = ""; // آی‌دی یا یوزرنیم کانال

            var keyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("👍 لایک", "like"),
                InlineKeyboardButton.WithCallbackData("👎 دیسلایک", "dislike")
            }
        });


            await _botClient.SendTextMessageAsync(
                chatId: channelUsernameOrId,
                text: $"📢 پیام جدید:\n\n{text}",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );

            return true;
        }


        public async Task<bool> EnterImageAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            await _botClient.SendTextMessageAsync(
                chatId.Value,
                "لطفاً عکس را وارد کنید.",
                cancellationToken: cancellationToken
                );

            await UserStateTools.SetStateAsync(chatId , nameof(ChannelState.WatingImage));

            return true;
        }
        public async Task<bool> HandleSendChannelImageAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message?.Chat?.Id;
            if (chatId == null || update.Message?.Photo == null || !update.Message.Photo.Any())
                return false;

            var photo = update.Message.Photo
                .OrderByDescending(p => p.FileSize)
                .FirstOrDefault();

            if (photo == null)
                return false;

            string? caption = "📸 عکس ارسال شده از سمت کاربر";

            string channelUsernameOrId = "";

            await _botClient.SendPhotoAsync(
                chatId: channelUsernameOrId,
                photo: new InputOnlineFile(photo.FileId),
                caption: caption,
                cancellationToken: cancellationToken
            );


            return true;
        }


        public async Task<bool> HandleSendChannelPollAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId =  UserStateTools.GetChatId(update);
            if (chatId == null)
                return false;

            string channelUsernameOrId = "";

            string question = "نظر شما درباره این ربات چیست؟";
            string[] options = new string[] { "عالی", "خوب", "متوسط", "ضعیف" };

            await _botClient.SendPollAsync(
                chatId: channelUsernameOrId,
                question: question,
                options: options,
                isAnonymous: true,
                allowsMultipleAnswers: false,
                cancellationToken: cancellationToken
            );

            return true;
        }


        public Task<bool> HandlShowMenuAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
