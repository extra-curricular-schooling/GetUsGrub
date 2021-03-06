﻿using CSULB.GetUsGrub.Models;
using System.Collections.Generic;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// The <c>CreateIndividualPostLogicValidationStrategy</c> class.
    /// Defines a strategy for validating models after processing business logic for creating an individual user.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 03/12/2018
    /// </para>
    /// </summary>
    public class CreateFirstTimeIndividualPostLogicValidationStrategy
    {
        private readonly UserAccount _userAccount;
        private readonly IList<SecurityQuestion> _securityQuestions;
        private readonly IList<SecurityAnswerSalt> _securityAnswerSalts;
        private readonly PasswordSalt _passwordSalt;
        private readonly UserClaims _userClaims;
        private readonly UserProfile _userProfile;
        private readonly SecurityQuestionValidator _securityQuestionValidator;
        private readonly SecurityAnswerSaltValidator _securityAnswerSaltValidator;
        private readonly UserValidator _userValidator;

        /// <summary>
        /// Constructor for CreateIndividualPostLogicValidationStrategy
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/13/2018
        /// </para>
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="securityQuestions"></param>
        /// <param name="securityAnswerSalts"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="userClaims"></param>
        /// <param name="userProfile"></param>
        public CreateFirstTimeIndividualPostLogicValidationStrategy(UserAccount userAccount, PasswordSalt passwordSalt, UserClaims userClaims, UserProfile userProfile, IList<SecurityQuestion> securityQuestions, IList<SecurityAnswerSalt> securityAnswerSalts)
        {
            _userAccount = userAccount;
            _securityQuestions = securityQuestions;
            _securityAnswerSalts = securityAnswerSalts;
            _passwordSalt = passwordSalt;
            _userClaims = userClaims;
            _userProfile = userProfile;
            _securityQuestionValidator = new SecurityQuestionValidator();
            _securityAnswerSaltValidator = new SecurityAnswerSaltValidator();
            _userValidator = new UserValidator();
        }

        /// <summary>
        /// The ExecuteStrategy method.
        /// Contains the logic to validate domain models for creating an individual user.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @updated: 03/13/2018
        /// </para>
        /// </summary>
        /// <returns>A boolean</returns>
        public ResponseDto<bool> ExecuteStrategy()
        {
            ResponseDto<bool> result;

            var validationWrappers = new List<IValidationWrapper>()
            {
                new ValidationWrapper<UserAccount>(_userAccount, "CreateUser", new UserAccountValidator()),
                new ValidationWrapper<PasswordSalt>(_passwordSalt, new PasswordSaltValidator()),
                new ValidationWrapper<UserProfile>(_userProfile, "CreateUser", new UserProfileValidator()),
                new ValidationWrapper<UserClaims>(_userClaims, new ClaimsValidator())
            };

            foreach (var securityQuestion in _securityQuestions)
            {
                validationWrappers.Add(new ValidationWrapper<SecurityQuestion>(securityQuestion, "CreateUser", _securityQuestionValidator));
            }

            foreach (var securityAnswerSalt in _securityAnswerSalts)
            {
                validationWrappers.Add(new ValidationWrapper<SecurityAnswerSalt>(securityAnswerSalt, _securityAnswerSaltValidator));
            }

            foreach (var validationWrapper in validationWrappers)
            {
                result = validationWrapper.ExecuteValidator();
                if (!result.Data)
                {
                    result.Error = GeneralErrorMessages.GENERAL_ERROR;
                    return result;
                }
            }

            // Validate username and display name are not equal
            result = _userValidator.CheckIfUsernameEqualsDisplayName(_userAccount.Username, _userProfile.DisplayName);
            if (result.Data)
            {
                result.Error = GeneralErrorMessages.GENERAL_ERROR;
                return result;
            }

            return new ResponseDto<bool>()
            {
                Data = true
            };
        }
    }
}
