namespace ChatApp.Models;
public record OutputMessage(
        string Message,
        string UserName,
        string Room,
        DateTimeOffset SentAt
    );
