using MassTransit;
using Messages;

static Task Handle(ConsumeContext<IMessage2> context)
{
    var headers = string.Join(
        ", ",
        context.Headers.GetAll().Select(header => $"{header.Key}: {header.Value}")
    );
    Console.WriteLine(
        $"[Subscriber C] Received IMessage2 [Text2={context.Message.Text2}] with headers: {headers}"
    );
    return Task.CompletedTask;
}

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");

    sbc.ReceiveEndpoint(
        "ksr-c",
        ec =>
        {
            ec.Handler<IMessage2>(Handle);
        }
    );
});

await bus.StartAsync();
Console.WriteLine("[Subscriber C] Started! Press Enter to quit...");

Console.ReadKey();
await bus.StopAsync();
