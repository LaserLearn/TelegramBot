namespace TelegramBot.model.Enum
{
    public enum UserBotState
    {
        None,
        Menu,
        Register,
        InGroup,
        InChannel,
        InSelenium,
        InPaging,
        InCheckMark,
        InNestedMenu
    }

    public enum MainMenuOption
    {
        Group,
        Channel,
        Register,
        Selenium,
        Paging,
        CheckMark,
        NestedMenu
    }


    public enum RegisterState
    {
        None,
        EnterName,
        WatingName,
        EnterEmail,
        WatingEmail,
        EnterAge,
        WatingAge,
        EnterRole,
        save,
        Done
    }

    public enum ChannelState
    {
        None,
        SendImage,
        WatingImage,
        SendFilm,
        WatingFilm,
        SendMessage,
        WatingMessage,
        SendPoll,
        WatingPoll,
        ChannelMenu,
        Done
    }
}
