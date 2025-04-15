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

await channel.QueueDeclareAsync(
    queue: "ksr-queue",
    durable: true,
    exclusive: false,
    autoDelete: false
);

var replyQueueName = (await channel.QueueDeclareAsync()).QueueName;
var consumer = new AsyncEventingBasicConsumer(channel);
await channel.BasicConsumeAsync(queue: replyQueueName, autoAck: true, consumer: consumer);

consumer.ReceivedAsync += (model, ea) =>
{
    Console.WriteLine($"[Sender] Received reply [{Encoding.UTF8.GetString(ea.Body.ToArray())}]");
    return Task.CompletedTask;
};

foreach (var i in Enumerable.Range(1, 10))
{
    var message = $"Message #{i}";

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: "ksr-queue",
        mandatory: false,
        body: Encoding.UTF8.GetBytes(message),
        basicProperties: new BasicProperties
        {
            Persistent = true,
            ReplyTo = replyQueueName,
            Headers = new Dictionary<string, object?>
            {
                { "sender", "Sender" },
                { "messageId", i.ToString() },
            },
        }
    );

    Console.WriteLine($"[Sender] Published [{message}]");
}

Console.WriteLine("[Sender] Press [enter] to exit.");
Console.ReadLine();
