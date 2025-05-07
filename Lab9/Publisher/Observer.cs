using System.Text;
using MassTransit;

namespace Publisher;

public class Observer : IConsumeObserver, IPublishObserver
{
    private readonly Dictionary<string, int> _received = new();
    private readonly Dictionary<string, int> _faulted = new();
    private readonly Dictionary<string, int> _consumed = new();
    private readonly Dictionary<string, int> _sent = new();

    public string GetStats()
    {
        var stats = new StringBuilder();
        stats.AppendLine("Received messages:");
        foreach (var kvp in _received)
        {
            stats.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        stats.AppendLine("Faulted messages:");
        foreach (var kvp in _faulted)
        {
            stats.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        stats.AppendLine("Consumed messages:");
        foreach (var kvp in _consumed)
        {
            stats.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        stats.AppendLine("Sent messages:");
        foreach (var kvp in _sent)
        {
            stats.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        return stats.ToString();
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
        where T : class
    {
        var messageType = context.Message.GetType().Name;
        if (!_faulted.ContainsKey(messageType))
        {
            _faulted[messageType] = 0;
        }
        _faulted[messageType]++;
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context)
        where T : class
    {
        var messageType = context.Message.GetType().Name;
        if (!_consumed.ContainsKey(messageType))
        {
            _consumed[messageType] = 0;
        }
        _consumed[messageType]++;
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context)
        where T : class
    {
        var messageType = context.Message.GetType().Name;
        if (!_sent.ContainsKey(messageType))
        {
            _sent[messageType] = 0;
        }
        _sent[messageType]++;
        return Task.CompletedTask;
    }

    public Task PreConsume<T>(ConsumeContext<T> context)
        where T : class
    {
        var messageType = context.Message.GetType().Name;
        if (!_received.ContainsKey(messageType))
        {
            _received[messageType] = 0;
        }
        _received[messageType]++;
        return Task.CompletedTask;
    }

    public Task PrePublish<T>(PublishContext<T> context)
        where T : class
    {
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception)
        where T : class
    {
        return Task.CompletedTask;
    }
}
