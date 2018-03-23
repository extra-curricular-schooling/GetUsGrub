﻿using CSULB.GetUsGrub.Models;
using FluentValidation;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// The <c>UserAccountValidator</c> class.
    /// Defines rules to validate a UserAuthenticationModel.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/11/2018
    /// </para>
    /// </summary>
    class UserAuthenticationModelValidator : AbstractValidator<UserAuthenticationModel>
    {
        public UserAuthenticationModelValidator()
        {
            RuleSet("UsernameAndPassword", () =>
            {
                RuleFor(x => x.Username)
                    .NotEmpty()
                    .NotNull()
                    .Matches(@"^[A-Za-z\d]+$");

                RuleFor(x => x.Password)
                    .NotEmpty()
                    .NotNull();
            });
        }
    }
}
