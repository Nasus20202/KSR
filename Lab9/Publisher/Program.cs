using MassTransit;
using MassTransit.Serialization;
using Messages;
using Publisher;

var messageCounter = 0;
var active = true;
Observer observer = new();

var controllerBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
{
    sbc.UseEncryptedSerializer(
        new AesCryptoStreamProvider(
            new CipherKeyProvider("19332819332819332819332819332819"),
            "1933281933281932"
        )
    );
    sbc.Host(
        new Uri("amqps://seal.lmq.cloudamqp.com/cfazakgh"),
        h =>
        {
            h.Username("cfazakgh");
            h.Password("GOrZUTRHi68EhknPqHQ0f2l8RttH69IA");
        }
    );

    sbc.ReceiveEndpoint(
        "ksr_controller",
        e =>
        {
            e.Handler<Ustaw>(context =>
            {
                active = context.Message.dziala;
                Console.WriteLine($"[Publisher] Received message {context.Message}");
                return Task.CompletedTask;
            });
        }
    );
});

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
        "ksr_a_response",
        e =>
        {
            e.UseMessageRetry(r =>
            {
                r.Interval(3, TimeSpan.FromMilliseconds(250));
            });
            e.Handler<OdpA>(context =>
            {
                Console.WriteLine($"[Publisher] Received message {context.Message}");

                if (Random.Shared.Next(0, 2) == 0)
                {
                    Console.WriteLine($"[Publisher] Throwing exception");
                    throw new Exception($"Processing {context.Message} failed");
                }

                return Task.CompletedTask;
            });
        }
    );

    sbc.ReceiveEndpoint(
        "ksr_b_response",
        e =>
        {
            e.UseMessageRetry(r =>
            {
                r.Interval(3, TimeSpan.FromMilliseconds(250));
            });
            e.Handler<OdpB>(context =>
            {
                Console.WriteLine($"[Publisher] Received message {context.Message}");

                if (Random.Shared.Next(0, 2) == 0)
                {
                    Console.WriteLine($"[Publisher] Throwing exception");
                    throw new Exception($"Processing {context.Message} failed");
                }

                return Task.CompletedTask;
            });
        }
    );
});

controllerBus.ConnectConsumeObserver(observer);
controllerBus.ConnectPublishObserver(observer);
bus.ConnectConsumeObserver(observer);
bus.ConnectPublishObserver(observer);

await controllerBus.StartAsync();
await bus.StartAsync();

var consoleTask = Task.Run(() =>
{
    Console.WriteLine("[Publisher] Press 's' to show stats");
    while (true)
    {
        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.S)
        {
            Console.WriteLine("[Publisher] Stats:");
            Console.WriteLine(observer.GetStats());
        }
    }
});

var publishTask = Task.Run(async () =>
{
    while (true)
    {
        if (active)
        {
            var message = new Publ(++messageCounter);
            Console.WriteLine($"[Publisher] Publishing message {message}");
            await bus.Publish(message);
        }
        else
        {
            Console.WriteLine("[Publisher] Not active");
        }

        await Task.Delay(1000);
    }
});

await Task.WhenAll(consoleTask, publishTask);
