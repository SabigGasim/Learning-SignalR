namespace ChatApp.Models;
public record UserMessage(
    User User,
    string Message,
    string Room,
    DateTimeOffset SentAt)
{
    public OutputMessage Output => new(Message, User.UserName, Room, SentAt);
}