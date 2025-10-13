namespace OpsPortal.Application.Security;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool RequiresRehash(string hash);

    bool VerifyPassword(string password, string hash);
}
