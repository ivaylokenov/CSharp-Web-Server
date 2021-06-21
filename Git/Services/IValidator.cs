namespace Git.Services
{
    using System.Collections.Generic;
    using Git.Models.Repositories;
    using Git.Models.Users;

    public interface IValidator
    {
        ICollection<string> ValidateUser(RegisterUserFormModel model);

        ICollection<string> ValidateRepository(CreateRepositoryFormModel model);
    }
}
