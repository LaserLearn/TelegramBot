using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.contract.Bot.register;
using TelegramBot.contract.Database.user;
using TelegramBot.model.Enum;
using TelegramBot.model.state;
using TelegramBot.model.user;
using TelegramBot.Tool;

namespace TelegramBot.handlers.register
{
    public class RegisterCommandHandler : IRegisterUserHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUser_Rep _user;
        public RegisterCommandHandler(TelegramBotClient botClient, IUser_Rep user)
        {
            _botClient = botClient;
            _user = user;
        }
        public async Task<bool> ShouldHandleAsync(Update update)
        {
            long? chatId = UserStateTools.GetChatId(update);

            if (chatId == null)
                return false;

            var step = await UserStateTools.GetStateAsync(chatId.Value);

            if (update.CallbackQuery != null && update.CallbackQuery.Data == "Register" ||
                await UserStateTools.GetMotherStateAsync(chatId) == nameof(UserBotState.Register))
            {
                await UserStateTools.SetMotherAsync(chatId: chatId, nameof(UserBotState.Register));
                return true;
            }
            return false;
        }

        public Task<bool> CheckUserExistsAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            long? chatId = UserStateTools.GetChatId(update);

            var step = await UserStateTools.GetStateAsync(chatId.Value);
            if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "register_role_admin":
                        await HandleRoleSelectionAsync(update, "Admin", cancellationToken);
                        return;

                    case "register_role_user":
                        await HandleRoleSelectionAsync(update, "User", cancellationToken);
                        return;
                    
                    case "Register":
                        await HandleReceiveNameAsync(update, cancellationToken);
                        return;
                    default:
                        await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "دستور ناشناخته");
                        break;
                }
            }
            switch (step)
            {

                case nameof(RegisterState.EnterName):
                    await HandleReceiveNameAsync(update, cancellationToken);
                    break;

                case nameof(RegisterState.WatingName):
                    await HandleWatingNameAsync(update, cancellationToken);

                    break;
                case nameof(RegisterState.EnterEmail):
                    await HandleReceiveEmailAsync(update, cancellationToken);
                    break;

                case nameof(RegisterState.WatingEmail):
                    await HandleWatingEmailAsync(update, cancellationToken);
                    break;

                case nameof(RegisterState.EnterAge):
                    await HandleReceiveAgeAsync(update, cancellationToken);
                    break;

                case nameof(RegisterState.WatingAge):
                    await HandleWatingAgeAsync(update, cancellationToken);
                    break;

                case nameof(RegisterState.EnterRole):
                    await HandleReceiveRoleAsync(update, cancellationToken);
                    break;
                
                case nameof(RegisterState.save):
                    await SaveUserAsync(update, cancellationToken);
                    break;
                
                case nameof(RegisterState.Done):
                    await ShowUserInfoAsync(update, cancellationToken);
                    break;
                default:
                    await HandleReceiveNameAsync(update, cancellationToken);
                    break;
            }
        }

        public async Task<bool> HandleReceiveAgeAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            await _botClient.SendTextMessageAsync(
                chatId.Value,
                "لطفاً سن خود را وارد کنید.",
                cancellationToken: cancellationToken
            );

            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.WatingAge));
            return true;
        }

        public async Task<bool> HandleWatingAgeAsync(Update update, CancellationToken cancellationToken)
        {
            var ageText = update.Message!.Text;
            var chatId = UserStateTools.GetChatId(update);
            var age = ageText.Trim();

            if (!int.TryParse(age, out var ageNumber))
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "سن وارد شده نامعتبر است. لطفاً فقط عدد وارد کنید.",
                    cancellationToken: cancellationToken
                );
                return false;
            }

            await UserStateTools.SetDataAsync(chatId, key: "age", value: age);
            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.EnterRole));

            await DispatchAsync(update, cancellationToken);

            return true;
        }


        public async Task<bool> HandleReceiveEmailAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            await _botClient.SendTextMessageAsync(
                chatId.Value,
                "لطفاً ایمیل خود را وارد کنید.",
                cancellationToken: cancellationToken
            );

            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.WatingEmail));
            return true;
        }
        public async Task<bool> HandleWatingEmailAsync(Update update, CancellationToken cancellationToken)
        {
            var email = update.Message!.Text;
            var chatId = UserStateTools.GetChatId(update);


            await UserStateTools.SetDataAsync(chatId, key: "email", value: email);
            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.EnterAge));

            await DispatchAsync(update, cancellationToken);

            return true;
        }


        public async Task<bool> HandleReceiveNameAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            await _botClient.SendTextMessageAsync(
                chatId.Value,
                "لطفاً نام خود را وارد کنید.",
                cancellationToken: cancellationToken
            );

            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.WatingName));
            return true;
        }
        public async Task<bool> HandleWatingNameAsync(Update update, CancellationToken cancellationToken)
        {
            var name = update.Message!.Text;
            var chatId = UserStateTools.GetChatId(update);


            await UserStateTools.SetDataAsync(chatId, key: "name", value: name);
            await UserStateTools.SetStateAsync(chatId, nameof(RegisterState.EnterEmail));

            await DispatchAsync(update, cancellationToken);

            return true;
        }


        public async Task<bool> HandleReceiveRoleAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("ادمین", "register_role_admin"),
                    InlineKeyboardButton.WithCallbackData("یوزر", "register_role_user")
                }
            });

            await _botClient.SendTextMessageAsync(
                chatId: chatId.Value,
                text: "لطفاً نقش خود را انتخاب کنید:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken
            );

            return true;
        }

        public async Task<bool> HandleRoleSelectionAsync(Update update, string role , CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            if (role == "Admin") await UserStateTools.SetDataAsync(chatId , "role" , Role_Em.Admin.ToString());
            else await UserStateTools.SetDataAsync(chatId, "role", Role_Em.user.ToString());

            await UserStateTools.SetStateAsync(chatId , RegisterState.save.ToString());
            await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "نقش انتخاب شد ✅", cancellationToken: cancellationToken);
            await SaveUserAsync(update , cancellationToken);
            return true;
        }

        public async Task<bool> SaveUserAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            var data = await UserStateTools.GetAllAsync(chatId.Value);
            if (data == null) return false;

            var name = data.Data.TryGetValue("name", out var nameValue) ? nameValue : null;
            var email = data.Data.TryGetValue("email", out var emailValue) ? emailValue : null;
            var ageStr = data.Data.TryGetValue("age", out var ageValue) ? ageValue : null;
            var role = data.Data.TryGetValue("role", out var roleValue) ? roleValue : null;

            int.TryParse(ageStr, out var age);

            var user_role = Role_Em.user;
            if(role == Role_Em.Admin.ToString())
            {
                user_role = Role_Em.Admin;
            }


            var newuser = new User_E()
            {
                age = age,
                email = email,
                telegram_id = chatId,
                user_name = name,
                role_Em = user_role
            };

            await _user.AddAsync(newuser);
            await UserStateTools.SetStateAsync(chatId , RegisterState.Done.ToString());
            await ShowUserInfoAsync(update , cancellationToken);
            return true;
        }

        public Task<bool> ShowRoleMenuAsync(Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ShowUserInfoAsync(Update update, CancellationToken cancellationToken)
        {
            var chatId = UserStateTools.GetChatId(update);
            if (chatId == null) return false;

            var user = await _user.GetBy_TelegramId(chatId ?? new long());

            var infoText = $"📋 اطلاعات ثبت‌شده:\n\n" +
                           $"👤 نام: {user.user_name}\n" +
                           $"📧 ایمیل: {user.email}\n" +
                           $"🎂 سن: {user.age}\n" +
                           $"🧑‍💼 نقش: {nameof(user.role_Em)}";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
        {
            InlineKeyboardButton.WithCallbackData("🔙 بازگشت", "register_back")
        }
            });

            await _botClient.SendTextMessageAsync(
                chatId: chatId.Value,
                text: infoText,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );


            return true;
        }
    }
}
