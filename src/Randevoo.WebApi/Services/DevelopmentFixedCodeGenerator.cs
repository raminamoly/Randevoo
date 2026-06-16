using Randevoo.Application.Interfaces.Auth;

namespace Randevoo.WebApi.Services;

public sealed class DevelopmentFixedCodeGenerator : ICodeGenerator
{
    public string GenerateNumericCode(int length)
    {
        const string code = "123456";
        return length <= code.Length ? code[..length] : code.PadLeft(length, '0');
    }

    public string GenerateToken() => Guid.NewGuid().ToString("N");
}
