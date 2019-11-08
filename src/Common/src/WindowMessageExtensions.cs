internal static class WindowMessageExtensions
{
    public static WindowMessage WindowMessage(this ref Message message)
        => (WindowMessage)message.Msg;

    public static bool Is(this ref Message message, WindowMessage windowMessage)
        => message.Msg == (int)windowMessage;

    public static bool IsMouseMessage(this ref Message message)
        => message.IsBetween(WM_MOUSEFIRST, WM_MOUSELAST);

    public static bool IsMouseMessage(this ref MSG message)
        => message.IsBetween(WM_MOUSEFIRST, WM_MOUSELAST);

    public static bool IsKeyMessage(this ref Message message)
        => message.IsBetween((WindowMessage)WM_KEYFIRST, (WindowMessage)WM_KEYLAST);

    public static bool IsKeyMessage(this ref MSG message)
        => message.IsBetween((WindowMessage)WM_KEYFIRST, (WindowMessage)WM_KEYLAST);

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref Message message,
        WindowMessage firstMessage,
        WindowMessage secondMessage)
        => message.Msg >= (int)firstMessage && message.Msg <= (int)secondMessage;

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref MSG message,
        WindowMessage firstMessage,
        WindowMessage secondMessage)
        => (uint)message.message >= (uint)firstMessage && (uint)message.message <= (uint)secondMessage;
}