namespace Application.Common.Events;

public record MessageSentEvent
{
    public Guid MessageId { get; init; }
    public Guid ChatId { get; init; }
    public string Content { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public Guid UserId { get; init; }
}