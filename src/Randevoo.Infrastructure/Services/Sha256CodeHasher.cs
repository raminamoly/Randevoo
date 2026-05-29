using System.Security.Cryptography;
using System.Text;
using Randevoo.Application.Interfaces.Auth;

namespace Randevoo.Infrastructure.Services;

public class Sha256CodeHasher : ICodeHasher
{
    public string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
