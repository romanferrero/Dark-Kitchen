namespace DarkKitchen.IBusinessLogic.IServices;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
