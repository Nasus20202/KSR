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

foreach (var i in Enumerable.Range(1, 10))
{
    var routingKey = i % 2 == 0 ? "abc.def" : "abc.xyz";
    var message = $"Message #{i}";

    await channel.BasicPublishAsync(
        exchange: "ksr-exchange",
        routingKey: routingKey,
        mandatory: false,
        body: Encoding.UTF8.GetBytes(message)
    );

    Console.WriteLine($"[Publisher] Published [{message}] with routing key [{routingKey}]");
}

Console.WriteLine("[Publisher] Press [enter] to exit.");
Console.ReadLine();
