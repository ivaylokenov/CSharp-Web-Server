namespace CarShop.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using CarShop.Models.Cars;
    using CarShop.Models.Issues;
    using CarShop.Models.Users;

    using static Data.DataConstants;

    public class Validator : IValidator
    {
        public ICollection<string> ValidateUser(RegisterUserFormModel user)
        {
            var errors = new List<string>();

            if (user.Username == null || user.Username.Length < UserMinUsername || user.Username.Length > DefaultMaxLength)
            {
                errors.Add($"Username '{user.Username}' is not valid. It must be between {UserMinUsername} and {DefaultMaxLength} characters long.");
            }

            if (user.Email == null || !Regex.IsMatch(user.Email, UserEmailRegularExpression))
            {
                errors.Add($"Email '{user.Email}' is not a valid e-mail address.");
            }

            if (user.Password == null || user.Password.Length < UserMinPassword || user.Password.Length > DefaultMaxLength)
            {
                errors.Add($"The provided password is not valid. It must be between {UserMinPassword} and {DefaultMaxLength} characters long.");
            }

            if (user.Password != null && user.Password.Any(x => x == ' '))
            {
                errors.Add($"The provided password cannot contain whitespaces.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and its confirmation are different.");
            }

            if (user.UserType != UserTypeClient && user.UserType != UserTypeMechanic)
            {
                errors.Add($"The user can be either a '{UserTypeClient}' or a '{UserTypeMechanic}'.");
            }

            return errors;
        }

        public ICollection<string> ValidateCar(AddCarFormModel car)
        {
            var errors = new List<string>();

            if (car.Model == null || car.Model.Length < CarModelMinLength || car.Model.Length > DefaultMaxLength)
            {
                errors.Add($"Model '{car.Model}' is not valid. It must be between {CarModelMinLength} and {DefaultMaxLength} characters long.");
            }

            if (car.Year < CarYearMinValue || car.Year > CarYearMaxValue)
            {
                errors.Add($"Year '{car.Year}' is not valid. It must be between {CarYearMinValue} and {CarYearMaxValue}.");
            }

            if (car.Image == null || !Uri.IsWellFormedUriString(car.Image, UriKind.Absolute))
            {
                errors.Add($"Image '{car.Image}' is not valid. It must be a valid URL.");
            }

            if (car.PlateNumber == null || !Regex.IsMatch(car.PlateNumber, CarPlateNumberRegularExpression))
            {
                errors.Add($"Plate number '{car.PlateNumber}' is not valid. It should be in 'XX0000XX' format.");
            }

            return errors;
        }

        public ICollection<string> ValidateIssue(AddIssueFormModel issue)
        {
            var errors = new List<string>();

            if (issue.CarId == null)
            {
                errors.Add($"Car ID cannot be empty.");
            }

            if (issue.Description.Length < IssueMinDescription)
            {
                errors.Add($"Description '{issue.Description}' is not valid. It must have more than {IssueMinDescription} characters.");
            }

            return errors;
        }
    }
}
