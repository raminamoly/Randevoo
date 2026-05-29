namespace Randevoo.Application.Interfaces.Auth;

public interface ICodeGenerator
{
    string GenerateNumericCode(int length);
    string GenerateToken();
}
