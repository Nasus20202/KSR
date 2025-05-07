using MassTransit;
using Messages;

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.Host(
        new Uri("amqps://seal.lmq.cloudamqp.com/cfazakgh"),
        h =>
        {
            h.Username("cfazakgh");
            h.Password("GOrZUTRHi68EhknPqHQ0f2l8RttH69IA");
        }
    );

    sbc.ReceiveEndpoint(
        "ksr_a",
        e =>
        {
            e.Handler<Publ>(async context =>
            {
                int number = context.Message.number;
                if (number % 2 == 0)
                {
                    return;
                }
                var response = new OdpA("abonent A");
                await context.RespondAsync(response);
                Console.WriteLine(
                    $"[Subscriber A] Received message {context.Message}, sending response {response}"
                );
                return;
            });
            e.Handler<Fault<OdpA>>(context =>
            {
                foreach (var exception in context.Message.Exceptions)
                {
                    Console.WriteLine($"[Subscriber A] Received exception: {exception.Message}");
                }
                return Task.CompletedTask;
            });
        }
    );
});

await bus.StartAsync();

Console.WriteLine("[Subscriber A] Press enter to exit");
Console.ReadLine();
