namespace NinjaBot.Domain.Extensions;

public static class AppMessageTypeExtensions
{
    public static string GetErrorMsg(this AppMessageType msg)
    {
        return msg switch
        {
            AppMessageType.InvalidRequest => "Invalid request",
            AppMessageType.NotFound => "Resource not found",
            AppMessageType.UnknownError => "Unknown error occurred",
            _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
        };
    }

    public static string GetErrorCodeValue(this AppMessageType msg) => $"{(int)msg}";

    public static string GetErrorCode(this AppMessageType msg)
    {
        int msgId = (int)msg;
        return $"BBO_{msgId}";
    }
}
