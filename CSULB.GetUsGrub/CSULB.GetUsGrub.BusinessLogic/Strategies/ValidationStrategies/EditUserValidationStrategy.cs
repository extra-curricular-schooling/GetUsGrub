﻿using CSULB.GetUsGrub.Models;
using System.Collections.Generic;

namespace CSULB.GetUsGrub.BusinessLogic
{
    public class EditUserValidationStrategy
    {
        private readonly EditUserDto _editUserDto; 
        private readonly EditUserDtoValidator _editUserDtoValidator;

        /// <summary>
        /// Defines a strategy for validating models before processing business logic 
        /// before preforming CRUD Operations.
        /// The constructor
        /// @author Angelica Salas Tovar, Jennifer Nguyen
        /// </summary>
        /// <param name="userAccountDto"></param>
        public EditUserValidationStrategy(EditUserDto editUserAccount)
        {
            _editUserDto = editUserAccount;
            _editUserDtoValidator = new EditUserDtoValidator();
        }

        public ResponseDto<bool> ExecuteStrategy()
        {
            // Will validate the username and new username
            var validationWrappers = new List<IValidationWrapper>()
            {
                new ValidationWrapper<EditUserDto>(_editUserDto, "EditUsername", _editUserDtoValidator),
            };

            // Goes through each validation in the validation wrapper.
            foreach (var validationWrapper in validationWrappers)
            {
                var result = validationWrapper.ExecuteValidator();
                if (!result.Data)
                {
                    return result;
                }
            }
            // Validator to check if user exists
            var userValidator = new UserValidator();
            if (userValidator.CheckIfUserExists(_editUserDto.Username).Data == false)
            {
                return new ResponseDto<bool>
                {
                    Data = false,
                    Error = ValidationErrorMessages.USER_DOES_NOT_EXIST
                };
            }

            //Check if new display name is null
            if(_editUserDto.NewDisplayName != null)
            {
                //Checks if username is equal to displayname
                if (userValidator.CheckIfUsernameEqualsDisplayName(_editUserDto.Username, _editUserDto.NewDisplayName).Data == true)
                {
                    return new ResponseDto<bool>
                    {
                        Data = false,
                        Error = ValidationErrorMessages.USERNAME_IS_EQUAL_TO_DISPLAYNAME
                    };
                }
            }

            return new ResponseDto<bool>()
            {
                Data = true
            };
        }
    }
}
