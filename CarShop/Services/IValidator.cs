namespace CarShop.Services
{
    using System.Collections.Generic;
    using CarShop.Models.Cars;
    using CarShop.Models.Issues;
    using CarShop.Models.Users;

    public interface IValidator
    {
        ICollection<string> ValidateUser(RegisterUserFormModel model);

        ICollection<string> ValidateCar(AddCarFormModel model);

        ICollection<string> ValidateIssue(AddIssueFormModel model);
    }
}
