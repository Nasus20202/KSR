using MassTransit;
using Messages;

var handler = new Handler();

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");

    sbc.ReceiveEndpoint(
        "ksr-b",
        ec =>
        {
            ec.Instance(handler);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[Subscriber B] Started! Press Enter to quit...");

Console.ReadKey();
await bus.StopAsync();

class Handler : IConsumer<IMessage3>
{
    private int _counter = 0;

    public Task Consume(ConsumeContext<IMessage3> context)
    {
        var headers = string.Join(
            ", ",
            context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
        );
        Console.WriteLine(
            $"[Subscriber B] Received IMessage3 [Text1={context.Message.Text1}, Text2={context.Message.Text2}] with headers: {headers}, counter: {++_counter}"
        );
        return Task.CompletedTask;
    }
}
