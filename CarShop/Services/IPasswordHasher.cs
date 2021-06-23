namespace CarShop.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}
