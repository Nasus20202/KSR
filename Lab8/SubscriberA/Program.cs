using MassTransit;
using Messages;

static Task Handle(ConsumeContext<IMessage1> context)
{
    var headers = string.Join(
        ", ",
        context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
    );
    Console.WriteLine(
        $"[Subscriber A] Received IMessage1 [Text1={context.Message.Text1}] with headers: {headers}"
    );
    return Task.CompletedTask;
}

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");

    sbc.ReceiveEndpoint(
        "ksr-a",
        ec =>
        {
            ec.Handler<IMessage1>(Handle);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[Subscriber A] Started! Press Enter to quit...");

Console.ReadKey();
await bus.StopAsync();
