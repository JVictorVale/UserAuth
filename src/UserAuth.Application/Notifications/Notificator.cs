using FluentValidation.Results;

namespace UserAuth.Application.Notifications;

public class Notificator : INotificator
{
    private readonly List<string> _notifications;
    private bool _notFoundResource;

    public Notificator()
    {
        _notifications = new List<string>();
    }

    public void Handle(string message)
    {
        if (_notFoundResource)
        {
            throw new InvalidOperationException("Não é possível chamar o Handle quando for NotFoundResourse!");
        }
        
        _notifications.Add(message);
    }

    public void Handle(List<ValidationFailure> failures)
    {
        if (HasNotification)
        {
            throw new InvalidOperationException("Não é possível chamar o HandleNotFoundResourse quando for Handle!");
        }
        
        failures.ForEach(error => Handle(error.ErrorMessage));
    }

    public void HandleNotFoundResource()
    {
        _notFoundResource = true;
    }

    public IEnumerable<string> GetNotifications() => _notifications;
    public bool HasNotification => _notifications.Any();

    public bool IsNotFoundResource => _notFoundResource;
}