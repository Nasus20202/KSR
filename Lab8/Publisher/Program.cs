using MassTransit;
using Messages;
using Publisher;

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");
});

await bus.StartAsync();
Console.WriteLine("[Publisher] Started! Press Enter to quit...");

async Task SendMessage<T>(T message, Dictionary<string, string> headers)
{
    var messageHeaders = string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}"));

    Console.WriteLine($"[Publisher] Sending [{message}] with headers [{messageHeaders}]");

    await bus.Publish(
        message!,
        context =>
        {
            foreach (var (key, value) in headers)
            {
                context.Headers.Set(key, value);
            }
        }
    );
}

while (true)
{
    foreach (var i in Enumerable.Range(0, 10))
    {
        IMessage1 message1 = new Message1 { Text1 = $"Message 1 #{i}" };
        var headers1 = new Dictionary<string, string>()
        {
            { "id", i.ToString() },
            { "hash", message1.GetHashCode().ToString() },
            { "receiver", "Subscriber A, Subscriber B" },
        };
        await SendMessage(message1, headers1);

        IMessage2 message2 = new Message2 { Text2 = $"Message 2 #{i}" };
        var headers2 = new Dictionary<string, string>()
        {
            { "id", i.ToString() },
            { "hash", message2.GetHashCode().ToString() },
            { "receiver", "Subscriber B, Subscriber C" },
        };
        await SendMessage(message2, headers2);

        IMessage3 message3 = new Message3 { Text1 = $"Message 3 #{i}", Text2 = $"Message 3 #{i}" };
        var headers3 = new Dictionary<string, string>()
        {
            { "id", i.ToString() },
            { "hash", message3.GetHashCode().ToString() },
            { "receiver", "Subscriber A, Subscriber B, Subscriber C" },
        };
        await SendMessage(message3, headers3);
        await Task.Delay(100);
    }

    Console.WriteLine("[Publisher] Press Enter to quit or any other key to continue...");
    var key = Console.ReadKey();
    if (key.Key == ConsoleKey.Enter)
        break;
}

await bus.StopAsync();
