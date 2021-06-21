namespace Git.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}
