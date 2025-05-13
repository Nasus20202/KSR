namespace Kontrakty;

public record StartZamowienia(int Ilosc, string Klient, Guid IdKorelacji);

public record PytanieOPotwierdzenie(int Ilosc, Guid IdKorelacji);

public record Potwierdzenie(Guid IdKorelacji);

public record BrakPotwierdzenia(Guid IdKorelacji);

public record PytanieOWolne(int Ilosc, Guid IdKorelacji);

public record OdpowiedzWolne(Guid IdKorelacji);

public record OdpowiedzWolneNegatywna(Guid IdKorelacji);

public record AkceptacjaZamowienia(int Ilosc, Guid IdKorelacji);

public record OdrzucenieZamowienia(int Ilosc, Guid IdKorelacji);

public record ZamowienieTimeout(Guid IdKorelacji);
