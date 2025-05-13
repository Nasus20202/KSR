using Kontrakty;
using MassTransit;

public class StanZamowieniaSaga : MassTransitStateMachine<StanZamowienia>
{
    public State? Oczekujacy { get; private set; }
    public State? Potwierdzony { get; private set; }
    public State? Odrzucony { get; private set; }

    public Event<StartZamowienia>? StartZamowieniaEvent { get; private set; }
    public Event<Potwierdzenie>? PotwierdzenieEvent { get; private set; }
    public Event<BrakPotwierdzenia>? BrakPotwierdzeniaEvent { get; private set; }
    public Event<OdpowiedzWolne>? OdpowiedzWolneEvent { get; private set; }
    public Event<OdpowiedzWolneNegatywna>? OdpowiedzWolneNegatywnaEvent { get; private set; }
    public Schedule<StanZamowienia, ZamowienieTimeout>? ZamowienieTimeoutSchedule
    {
        get;
        private set;
    }

    public StanZamowieniaSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartZamowieniaEvent, x => x.CorrelateById(c => c.Message.IdKorelacji));
        Event(() => PotwierdzenieEvent, x => x.CorrelateById(c => c.Message.IdKorelacji));
        Event(() => BrakPotwierdzeniaEvent, x => x.CorrelateById(c => c.Message.IdKorelacji));
        Event(() => OdpowiedzWolneEvent, x => x.CorrelateById(c => c.Message.IdKorelacji));
        Event(() => OdpowiedzWolneNegatywnaEvent, x => x.CorrelateById(c => c.Message.IdKorelacji));
        Schedule(
            () => ZamowienieTimeoutSchedule,
            x => x.TimeoutTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromSeconds(10);
                s.Received = x => x.CorrelateById(c => c.Message.IdKorelacji);
            }
        );

        Initially(
            When(StartZamowieniaEvent)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Zamówienie rozpoczęte przez klienta {context.Message.Klient}, ilość: {context.Message.Ilosc}"
                    );
                    context.Saga.Ilosc = context.Message.Ilosc;
                    context.Saga.Klient = context.Message.Klient;
                })
                .TransitionTo(Oczekujacy)
                .Schedule(
                    ZamowienieTimeoutSchedule,
                    context => new ZamowienieTimeout(context.Saga.CorrelationId)
                )
                .ThenAsync(async context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Wysyłam zapytanie do hurtowni o dostępność {context.Saga.Ilosc} sztuk."
                    );
                    var endpoint = await context.GetSendEndpoint(new Uri("queue:hurtownia"));
                    await endpoint.Send(
                        new PytanieOWolne(context.Saga.Ilosc, context.Saga.CorrelationId)
                    );

                    Console.WriteLine(
                        $"[Sklep] Wysyłam zapytanie do klienta {context.Saga.Klient} o potwierdzenie zamówienia."
                    );
                    endpoint = await context.GetSendEndpoint(
                        new Uri($"queue:klient-{context.Saga.Klient}")
                    );
                    await endpoint.Send(
                        new PytanieOPotwierdzenie(context.Saga.Ilosc, context.Saga.CorrelationId)
                    );
                })
        );

        During(
            Oczekujacy,
            When(PotwierdzenieEvent)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Klient {context.Saga.Klient} potwierdził zamówienie na {context.Saga.Ilosc} sztuk."
                    );
                    context.Saga.CzyKlientPotwierdzil = true;
                })
                .ThenAsync(SprobujZakonczyc),
            When(BrakPotwierdzeniaEvent)
                .TransitionTo(Odrzucony)
                .ThenAsync(async context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Klient {context.Saga.Klient} nie potwierdził zamówienia na {context.Saga.Ilosc} sztuk. Wysyłam do hurtowni informację o odrzuceniu zamówienia."
                    );
                    var endpoint = await context.GetSendEndpoint(new Uri($"queue:hurtownia"));
                    await endpoint.Send(
                        new OdrzucenieZamowienia(context.Saga.Ilosc, context.Saga.CorrelationId)
                    );
                }),
            When(OdpowiedzWolneEvent)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Hurtownia potwierdziła dostępność {context.Saga.Ilosc} sztuk."
                    );
                    context.Saga.CzyHurtowniaPotwierdzila = true;
                })
                .ThenAsync(SprobujZakonczyc),
            When(OdpowiedzWolneNegatywnaEvent)
                .TransitionTo(Odrzucony)
                .ThenAsync(async context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Hurtownia nie potwierdziła dostępności {context.Saga.Ilosc} sztuk. Wysyłam do klienta informację o odrzuceniu zamówienia."
                    );
                    var endpoint = await context.GetSendEndpoint(
                        new Uri($"queue:klient-{context.Saga.Klient}")
                    );
                    await endpoint.Send(
                        new OdrzucenieZamowienia(context.Saga.Ilosc, context.Saga.CorrelationId)
                    );
                }),
            When(ZamowienieTimeoutSchedule!.Received)
                .TransitionTo(Odrzucony)
                .ThenAsync(async context =>
                {
                    Console.WriteLine(
                        $"[Sklep] Zamówienie wygasło. Wysyłam do hurtowni i klienta informację o odrzuceniu zamówienia."
                    );
                    var kontrakt = new OdrzucenieZamowienia(
                        context.Saga.Ilosc,
                        context.Saga.CorrelationId
                    );
                    var endpoint = await context.GetSendEndpoint(new Uri($"queue:hurtownia"));
                    await endpoint.Send(kontrakt);
                    var endpoint2 = await context.GetSendEndpoint(
                        new Uri($"queue:klient-{context.Saga.Klient}")
                    );
                    await endpoint2.Send(kontrakt);
                })
        );
    }

    private async Task SprobujZakonczyc(BehaviorContext<StanZamowienia> context)
    {
        if (context.Saga.CzyKlientPotwierdzil && context.Saga.CzyHurtowniaPotwierdzila)
        {
            Console.WriteLine(
                $"[Sklep] Zamówienie na {context.Saga.Ilosc} sztuk zostało potwierdzone przez klienta {context.Saga.Klient} i hurtownię. Przechodzę do stanu potwierdzonego."
            );
            context.Saga.CurrentState = nameof(Potwierdzony);

            var kontrakt = new AkceptacjaZamowienia(context.Saga.Ilosc, context.Saga.CorrelationId);
            var endpoint = await context.GetSendEndpoint(
                new Uri($"queue:klient-{context.Saga.Klient}")
            );
            await endpoint.Send(kontrakt);

            endpoint = await context.GetSendEndpoint(new Uri($"queue:hurtownia"));
            await endpoint.Send(kontrakt);
        }
    }
}
