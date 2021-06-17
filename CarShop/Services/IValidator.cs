namespace CarShop.Services
{
    using CarShop.Models.Cars;
    using CarShop.Models.Users;
    using System.Collections.Generic;

    public interface IValidator
    {
        ICollection<string> ValidateUser(RegisterUserFormModel model);

        ICollection<string> ValidateCar(AddCarFormModel model);
    }
}
