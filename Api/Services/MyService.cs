namespace Api.Services;

public class MyService : IMyService
{
    public string Welcome(string name)
    {
        return $"Welcome, {name}\n\n{DateTime.UtcNow}";
    }
}
