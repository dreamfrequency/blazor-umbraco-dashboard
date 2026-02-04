namespace BlazorUmbracoDashboard.Web.Services;

public enum ToastLevel
{
    Success,
    Error,
    Info,
    Warning
}

public class ToastMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ToastLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ToastService
{
    public event Action? OnChange;
    private readonly List<ToastMessage> _toasts = new();

    public IReadOnlyList<ToastMessage> Toasts => _toasts.AsReadOnly();

    public void ShowSuccess(string message) => Show(ToastLevel.Success, message);
    public void ShowError(string message) => Show(ToastLevel.Error, message);
    public void ShowInfo(string message) => Show(ToastLevel.Info, message);
    public void ShowWarning(string message) => Show(ToastLevel.Warning, message);

    private void Show(ToastLevel level, string message)
    {
        _toasts.Add(new ToastMessage { Level = level, Message = message });
        OnChange?.Invoke();
    }

    public void Remove(Guid id)
    {
        _toasts.RemoveAll(t => t.Id == id);
        OnChange?.Invoke();
    }
}
