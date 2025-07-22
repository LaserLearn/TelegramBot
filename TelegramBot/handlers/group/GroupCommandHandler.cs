using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.contract.Bot.group;
using TelegramBot.model.Enum;
using TelegramBot.Tool;

namespace TelegramBot.handlers.group
{
    public class GroupCommandHandler : IGroupHandler
    {
        private readonly TelegramBotClient _botClient;

        public GroupCommandHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<bool> ShouldHandleAsync(Update update)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            if (update.Type == UpdateType.Message &&
                (update.Message.Chat.Type == ChatType.Group || update.Message.Chat.Type == ChatType.Supergroup))
                return true;

            if (update.Type == UpdateType.CallbackQuery)
            {
                var chat = update.CallbackQuery.Message?.Chat;
                if (chat != null && (chat.Type == ChatType.Group || chat.Type == ChatType.Supergroup))
                    return true;
            }

            return false;
        }
        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;

                if (message.NewChatMembers != null && message.NewChatMembers.Any())
                {
                    await HandleWelcomeAsync(update, cancellationToken);
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text))
                {
                    if (Regex.IsMatch(message.Text, @"(http[s]?:\/\/|t\.me\/|@[\w\d_]+|\w+\.(ir|com|org|net))", RegexOptions.IgnoreCase))
                    {
                        await HandleLinkLockAsync(update, cancellationToken);
                        return;
                    }
                }

                if (message.Voice != null)
                {
                    await HandleVoiceLockAsync(update, cancellationToken);
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text) && message.Text.Trim().ToLower() == "pin")
                {
                    await HandleAutoPinAsync(update, cancellationToken);
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text) && message.Text.ToLower().Contains("ربات سی شارپ"))
                {
                    await HandleAutoReplyAsync(update, cancellationToken);
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text) && message.ReplyToMessage != null)
                {
                    var text = message.Text.Trim();
                    if (text.Contains("اخراج") || text.Contains("سکوت") || text.Contains("هشدار"))
                    {
                        await HandleModerationAsync(update, cancellationToken);
                        return;
                    }
                }
            }

        }

        public async Task HandleAutoPinAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message == null)
                return;

            var message = update.Message;

            if (message.Chat.Type != ChatType.Supergroup && message.Chat.Type != ChatType.Group)
                return;

            if (message.ReplyToMessage == null || string.IsNullOrWhiteSpace(message.Text))
                return;

            if (message.Text.Trim().ToLower() != "pin")
                return;

            var admins = await _botClient.GetChatAdministratorsAsync(message.Chat.Id, cancellationToken);

            var senderIsAdmin = admins.Any(a =>
                a.User.Id == message.From.Id &&
                (
                    a is ChatMemberOwner ||
                    (a is ChatMemberAdministrator admin && admin.CanPinMessages == true)
                )
            );

            if (!senderIsAdmin)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "شما اجازه پین کردن پیام را ندارید.",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken
                );
                return;
            }

            await _botClient.PinChatMessageAsync(
                chatId: message.Chat.Id,
                messageId: message.ReplyToMessage.MessageId,
                cancellationToken: cancellationToken
            );
        }


        public async Task HandleAutoReplyAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message == null)
                return;

            var message = update.Message;

            if (message.Chat.Type != ChatType.Group && message.Chat.Type != ChatType.Supergroup)
                return;

            if (string.IsNullOrWhiteSpace(message.Text))
                return;

            if (message.Text.ToLower().Contains("ربات سی شارپ"))
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: " با من کاری داشتی؟ 🤖",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken
                );
            }
        }


        public async Task HandleLinkLockAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message == null || update.Message.Text == null)
                return;

            var message = update.Message;

            if (message.Chat.Type != ChatType.Group && message.Chat.Type != ChatType.Supergroup)
                return;

            string text = message.Text;

            var linkPattern = @"(http[s]?:\/\/|t\.me\/|@[\w\d_]+|\w+\.(ir|com|org|net))";
            if (Regex.IsMatch(text, linkPattern, RegexOptions.IgnoreCase))
            {
                var user = message.From;
                var name = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName :
                           !string.IsNullOrEmpty(user.Username) ? "@" + user.Username :
                           "کاربر";

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{name} لینک نفرست 🚫",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken
                );
            }
        }


        public async Task HandleModerationAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message == null || update.Message.ReplyToMessage == null)
                return;

            var message = update.Message;
            var commandText = message.Text?.Trim();
            var chatId = message.Chat.Id;

            if (message.Chat.Type != ChatType.Group && message.Chat.Type != ChatType.Supergroup)
                return;

            var fromUser = message.From;
            var adminStatus = await _botClient.GetChatMemberAsync(chatId, fromUser.Id, cancellationToken);
            if (adminStatus.Status != ChatMemberStatus.Administrator && adminStatus.Status != ChatMemberStatus.Creator)
                return;

            var targetUser = message.ReplyToMessage.From;

            if (string.IsNullOrWhiteSpace(commandText))
                return;

            if (commandText.Contains("اخراج"))
            {
                await _botClient.BanChatMemberAsync(chatId, targetUser.Id, cancellationToken: cancellationToken);
                await _botClient.SendTextMessageAsync(chatId, $"کاربر {targetUser.FirstName} از گروه اخراج شد.", cancellationToken: cancellationToken);
            }
            else if (commandText.Contains("هشدار"))
            {
                await _botClient.SendTextMessageAsync(chatId, $"🚨 {targetUser.FirstName} حواست رو جمع کن! این یه هشدار بود.", cancellationToken: cancellationToken);
            }
            else if (commandText.Contains("سکوت"))
            {
                var until = DateTime.UtcNow.AddHours(1);
                var permissions = new ChatPermissions
                {
                    CanSendMessages = false,
                    CanSendMediaMessages = false,
                    CanSendPolls = false,
                    CanSendOtherMessages = false,
                    CanAddWebPagePreviews = false,
                    CanChangeInfo = false,
                    CanInviteUsers = false,
                    CanPinMessages = false
                };

                await _botClient.RestrictChatMemberAsync(
                    chatId: chatId,
                    userId: targetUser.Id,
                    permissions: permissions,
                    untilDate: until,
                    cancellationToken: cancellationToken
                );

                await _botClient.SendTextMessageAsync(chatId, $"🔇 {targetUser.FirstName} برای یک ساعت سایلنت شد.", cancellationToken: cancellationToken);
            }
        }


        public async Task HandleVoiceLockAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message?.Voice == null)
                return;

            var chatId = update.Message.Chat.Id;
            var fromUser = update.Message.From;

            if (fromUser == null || fromUser.IsBot)
                return;

            string userName = string.IsNullOrEmpty(fromUser.Username)
                ? $"{fromUser.FirstName} {fromUser.LastName}".Trim()
                : $"@{fromUser.Username}";

            await _botClient.DeleteMessageAsync(
                chatId,
                update.Message.MessageId,
                cancellationToken
            );

            await _botClient.SendTextMessageAsync(
                chatId,
                $"{userName} عزیز، ارسال ویس در این گروه مجاز نیست ❌",
                cancellationToken: cancellationToken
            );
        }


        public async Task HandleWelcomeAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update.Message?.NewChatMembers == null)
                return;

            var newMembers = update.Message.NewChatMembers;
            var chatId = update.Message.Chat.Id;

            foreach (var member in newMembers)
            {
                var name = string.IsNullOrWhiteSpace(member.FirstName) ? "کاربر جدید" : member.FirstName;

                await _botClient.SendTextMessageAsync(
                    chatId,
                    $"🎉 خوش اومدی {name} عزیز به گروه!",
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
