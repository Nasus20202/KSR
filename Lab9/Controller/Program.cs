using MassTransit;
using MassTransit.Serialization;
using Messages;

var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
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
});

Console.WriteLine("[Controller] Press 'S' to start publishing messages, 'T' to stop.");

while (true)
{
    var key = Console.ReadKey();
    var message = key.Key switch
    {
        ConsoleKey.S => new Ustaw(true),
        ConsoleKey.T => new Ustaw(false),
        _ => null,
    };
    if (message is not null)
    {
        Console.WriteLine($"[Controller] Publishing message {message}");
        await bus.Publish(
            message,
            context =>
            {
                context.Headers.Set(
                    EncryptedMessageSerializer.EncryptionKeyHeader,
                    Guid.NewGuid().ToString()
                );
            }
        );
    }
}
