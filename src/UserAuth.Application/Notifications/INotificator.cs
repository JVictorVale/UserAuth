using FluentValidation.Results;

namespace UserAuth.Application.Notifications;

public interface INotificator
{
    bool HasNotification { get; }
    bool IsNotFoundResource { get; }

    void Handle(string message);
    void Handle(List<ValidationFailure> failures);
    void HandleNotFoundResource();
    IEnumerable<string> GetNotifications();
}