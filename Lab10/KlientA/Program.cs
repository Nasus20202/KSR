using Kontrakty;
using MassTransit;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");

    cfg.ReceiveEndpoint(
        "klient-a",
        e =>
        {
            e.Handler<PytanieOPotwierdzenie>(async context =>
            {
                Console.WriteLine(
                    $"[Klient A] Otrzymano pytanie o potwierdzenie na {context.Message.Ilosc} sztuk. Potwierdzić? (t/n)"
                );
                var klawisz = Console.ReadKey();
                if (klawisz.Key == ConsoleKey.T)
                {
                    Console.WriteLine(
                        $"[Klient A] Potwierdzam zamówienie na {context.Message.Ilosc} sztuk."
                    );
                    await context.RespondAsync(new Potwierdzenie(context.Message.IdKorelacji));
                }
                else
                {
                    Console.WriteLine(
                        $"[Klient A] Nie potwierdzam zamówienia na {context.Message.Ilosc} sztuk."
                    );
                    await context.RespondAsync(new BrakPotwierdzenia(context.Message.IdKorelacji));
                }
            });

            e.Handler<AkceptacjaZamowienia>(context =>
            {
                Console.WriteLine(
                    $"[Klient A] Zamówienie na {context.Message.Ilosc} sztuk zostało zaakceptowane."
                );
                return Task.CompletedTask;
            });

            e.Handler<OdrzucenieZamowienia>(context =>
            {
                Console.WriteLine(
                    $"[Klient A] Zamówienie na {context.Message.Ilosc} sztuk zostało odrzucone."
                );
                return Task.CompletedTask;
            });
        }
    );
});

await bus.StartAsync();

Console.WriteLine("[Klient A] Naciśnij O, aby wysłać zamówienie. Naciśnij Enter, aby zakończyć.");

while (true)
{
    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Enter)
        break;
    if (key.Key != ConsoleKey.O)
        continue;

    Console.WriteLine("[Klient A] Wpisz ilość sztuk do zamówienia");
    var input = Console.ReadLine();
    if (int.TryParse(input, out var ilosc))
    {
        var idKorelacji = Guid.NewGuid();
        var endpoint = await bus.GetSendEndpoint(new Uri("queue:sklep"));
        await endpoint.Send(new StartZamowienia(ilosc, "a", idKorelacji));
    }
    else
    {
        Console.WriteLine("[Klient A] Niepoprawna ilość sztuk.");
    }

    await Task.Delay(1000);
}
