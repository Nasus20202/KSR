using Kontrakty;
using MassTransit;

int wolne = 0;
int zarezerwowane = 0;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqps://cfazakgh:GOrZUTRHi68EhknPqHQ0f2l8RttH69IA@seal.lmq.cloudamqp.com/cfazakgh");

    cfg.ReceiveEndpoint(
        "hurtownia",
        e =>
        {
            e.Handler<PytanieOWolne>(async context =>
            {
                if (wolne >= context.Message.Ilosc)
                {
                    wolne -= context.Message.Ilosc;
                    zarezerwowane += context.Message.Ilosc;
                    Console.WriteLine(
                        $"[Hurtownia] Klient zarezerwował {context.Message.Ilosc} sztuk. Wolne: {wolne}, Zarezerwowane: {zarezerwowane}"
                    );
                    await context.RespondAsync(new OdpowiedzWolne(context.Message.IdKorelacji));
                }
                else
                {
                    Console.WriteLine(
                        $"[Hurtownia] Liczba wolnych jednostek ({wolne}) jest mniejsza niż zamówiona ({context.Message.Ilosc}). Nie można zrealizować zamówienia. Wolne: {wolne}, Zarezerwowane: {zarezerwowane}"
                    );
                    await context.RespondAsync(
                        new OdpowiedzWolneNegatywna(context.Message.IdKorelacji)
                    );
                }
            });

            e.Handler<AkceptacjaZamowienia>(context =>
            {
                zarezerwowane -= context.Message.Ilosc;
                Console.WriteLine(
                    $"[Hurtownia] Zamówienie na {context.Message.Ilosc} sztuk zostało zaakceptowane. Wolne: {wolne}, Zarezerwowane: {zarezerwowane}"
                );
                return Task.CompletedTask;
            });

            e.Handler<OdrzucenieZamowienia>(context =>
            {
                wolne += context.Message.Ilosc;
                zarezerwowane -= context.Message.Ilosc;
                Console.WriteLine(
                    $"[Hurtownia] Zamówienie na {context.Message.Ilosc} sztuk zostało odrzucone. Wolne: {wolne}, Zarezerwowane: {zarezerwowane}"
                );
                return Task.CompletedTask;
            });
        }
    );
});

await bus.StartAsync();

Console.WriteLine("[Hurtownia] Wpisz ilość sztuk do dodania (lub wciśnij Enter, aby zakończyć):");
while (true)
{
    var input = Console.ReadLine();
    if (input?.ToLower() == "")
        break;

    if (int.TryParse(input, out var ilosc))
    {
        wolne += ilosc;
        Console.WriteLine(
            $"[Hurtownia] Dodano {ilosc} sztuk do stanu magazynowego. Wolne: {wolne}, Zarezerwowane: {zarezerwowane}"
        );
    }
}

await bus.StopAsync();
