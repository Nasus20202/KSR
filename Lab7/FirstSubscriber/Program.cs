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

await channel.ExchangeDeclareAsync(exchange: "ksr-exchange", type: ExchangeType.Topic);

var queueName = (await channel.QueueDeclareAsync()).QueueName;
await channel.QueueBindAsync(queueName, "ksr-exchange", "abc.*");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    Console.WriteLine(
        $"[FirstSubscriber] Received message [{Encoding.UTF8.GetString(ea.Body.ToArray())}] with routing key [{ea.RoutingKey}]"
    );

    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine("[FirstSubscriber] Press [enter] to exit.");
Console.ReadLine();
