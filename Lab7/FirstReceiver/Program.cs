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
        $"[FirstReceiver] Received [{message}], headers: {
        string.Join(", ", ea.BasicProperties.Headers!.Select(h => $"{h.Key}: {Encoding.UTF8.GetString((byte[])h.Value!)}"))
    }"
    );

    await Task.Delay(2000);
    await channel.BasicAckAsync(ea.DeliveryTag, false);
};

await channel.BasicConsumeAsync(queue: "ksr-queue", autoAck: false, consumer: consumer);

Console.WriteLine("[FirstReceiver] Press [enter] to exit.");
Console.ReadLine();
