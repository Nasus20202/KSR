using System.Text;
using MassTransit.Serialization;

public class CipherKey : SymmetricKey
{
    public byte[]? IV { get; set; }
    public byte[]? Key { get; set; }
}

public record CipherKeyProvider(string key) : ISymmetricKeyProvider
{
    public bool TryGetKey(string id, out SymmetricKey symmetricKey)
    {
        var cipherKey = new CipherKey
        {
            IV = Encoding.ASCII.GetBytes(id.Substring(0, 16)),
            Key = Encoding.ASCII.GetBytes(key),
        };
        symmetricKey = cipherKey;
        return true;
    }
}
