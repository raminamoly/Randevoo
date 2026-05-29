namespace Randevoo.Application.Interfaces.Auth;

public interface ICodeHasher
{
    string Hash(string value);
}
