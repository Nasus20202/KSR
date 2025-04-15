using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    UserName = "cfazakgh",
    Password = "GOrZUTRHi68EhknPqHQ0f2l8RttH69IA",
    HostName = "seal-01.lmq.cloudamqp.com",
    VirtualHost = "cfazakgh",
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.BasicQosAsync(0, 1, false);
await channel.QueueDeclareAsync(
    queue: "ksr-queue",
    durable: true,
    exclusive: false,
    autoDelete: false
);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine(
        $"[SecondReceiver] Received [{message}], headers: {
        string.Join(", ", ea.BasicProperties.Headers!.Select(h => $"{h.Key}: {Encoding.UTF8.GetString((byte[])h.Value!)}"))
    }"
    );

    if (ea.BasicProperties.ReplyTo is not null)
    {
        Console.WriteLine($"[SecondReceiver] Sending reply to [{ea.BasicProperties.ReplyTo}]");
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: ea.BasicProperties.ReplyTo,
            mandatory: false,
            body: Encoding.UTF8.GetBytes($"Reply to [{message}] from [SecondReceiver]")
        );
    }

    await Task.Delay(2000);
    await channel.BasicAckAsync(ea.DeliveryTag, false);
};

await channel.BasicConsumeAsync(queue: "ksr-queue", autoAck: false, consumer: consumer);

Console.WriteLine("[SecondReceiver] Press [enter] to exit.");
Console.ReadLine();
