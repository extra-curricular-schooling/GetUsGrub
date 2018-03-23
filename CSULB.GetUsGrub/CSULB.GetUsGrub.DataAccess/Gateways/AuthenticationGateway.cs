﻿using CSULB.GetUsGrub.Models;
using System;
using System.Linq;

namespace CSULB.GetUsGrub.DataAccess
{

    /// <summary>
    /// Gateway to Access information from the DB for authentication purposes.
    /// 
    /// @Created by: Ahmed AlSadiq, Jennifer Nguyen
    /// @Last Updated: 03/22/18
    /// </summary>
    public class AuthenticationGateway : IDisposable
    {

        /// <summary>
        /// 
        /// GetUserAccount
        /// 
        /// Gets the Useraccount using the incoming UserAccountDTO.Username to map 
        /// to the UserAccount.UserName in the DataBase
        /// 
        /// </summary>
        /// <param name="incomingUser"></param>
        /// <returns> 
        /// UserAccount object to the Manager
        /// 
        /// </returns>
        public ResponseDto<UserAccount> GetUserAccount(UserAuthenticationModel incomingUser)
        {
            using (var authenticationContext = new AuthenticationContext())
            {
                // Looking for the User matching the incoming Username 
                var userFromDataBase = (from user in authenticationContext.UserAccounts
                    where user.Username == incomingUser.Username
                    select user).SingleOrDefault();
                return new ResponseDto<UserAccount>
                {
                    Data = userFromDataBase
                };
            }
        }

        /// <summary>
        ///
        ///  The StoreAuthenticationToken Method
        ///
        ///  Saves the Authentication Token created for the user onto the DataBase
        /// </summary>
        /// <param name="incomingAuthenticationToken">
        /// </param>
        public void StoreAuthenticationToken(AuthenticationToken incomingAuthenticationToken)
        {
            using (var authenticationContext = new AuthenticationContext())
            {
                using (var dbContextTransaction = authenticationContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Get the UserId using Username
                        var userId = (from account in authenticationContext.UserAccounts
                            where account.Username == incomingAuthenticationToken.Username
                            select account.Id).SingleOrDefault();

                        // Adding the Username to the Token as the Token ID
                        incomingAuthenticationToken.Id = userId;

                        // Adding the Token to the DataBase
                        authenticationContext.AuthenticationTokens.Add(incomingAuthenticationToken);

                        // Commiting the trasaction to the Database
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Rolls back the changes saved in the transaction
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///  
        /// GetAuthenticationToken
        /// 
        /// Gets the AuthenticationToken using the incoming TokenString to map 
        /// to the Token String in the DataBas
        /// </summary>
        /// <param name="incomingAuthenticationToken"></param>
        /// <returns></returns>
        public ResponseDto<AuthenticationToken> GetAuthenticationToken(AuthenticationToken incomingAuthenticationToken)
        {
            using (var authenticationContext = new AuthenticationContext())
            {
                var userId = (from account in authenticationContext.UserAccounts
                    where account.Username == incomingAuthenticationToken.Username
                    select account.Id).SingleOrDefault();

                // Looking for the a Token that matches the incoming Token
                var databaseToken = (from token in authenticationContext.AuthenticationTokens
                    where token.Id == userId
                    select token).SingleOrDefault();

                return new ResponseDto<AuthenticationToken>
                {
                    Data = databaseToken
                };
            }
        }

        /// <summary>
        /// GetUserPasswordSalt Uses the UserAccount.Id to map to the Salt.Is on the DB
        /// 
        /// </summary>
        /// <param name="userFromDataBase"></param>
        /// <returns>
        /// 
        /// string with the value of the salt associated with the user to the Manager
        /// 
        /// </returns>
        public ResponseDto<PasswordSaltDto> GetUserPasswordSalt(UserAccount userFromDataBase)
        {
            using (var userContext = new AuthenticationContext())
            {
                var salt = (from salts in userContext.PasswordSalts
                    where salts.Id == userFromDataBase.Id
                    select salts.Salt).SingleOrDefault();
                var passwordSaltDto = new PasswordSaltDto();
                passwordSaltDto.Salt = salt;
                return new ResponseDto<PasswordSaltDto>
                {
                    Data = passwordSaltDto
                };
            }
        }

        // TODO: @Ahmed Need to comment this [-Jenn]
        public ResponseDto<AuthenticationToken> GetExpirationTime(AuthenticationToken incomingAuthenticationToken)
        {
            AuthenticationToken tokenWithNewExpirationDate;
            using (var userContext = new AuthenticationContext())
            {
                tokenWithNewExpirationDate = (from token in userContext.AuthenticationTokens
                    where token.TokenString == incomingAuthenticationToken.TokenString
                    select token).SingleOrDefault();
            }

            return new ResponseDto<AuthenticationToken>
            {
                Data = tokenWithNewExpirationDate
            };
        }

        // TODO: @Jenn Comment this method and unit test [-Jenn]
        public ResponseDto<SsoToken> GetSsoToken(string token)
        {
            using (var authenticationContext = new AuthenticationContext())
            {
                try
                {
                    var ssoToken = (from storedToken in authenticationContext.SsoTokens
                        where storedToken.Token == token
                        select storedToken).FirstOrDefault();

                    // Return a ResponseDto with a UserAccount model
                    return new ResponseDto<SsoToken>()
                    {
                        Data = ssoToken
                    };
                }
                catch (Exception)
                {
                    return new ResponseDto<SsoToken>()
                    {
                        Data = new SsoToken(token),
                        Error = "Something went wrong. Please try again later."
                    };
                }
            }
        }

        // TODO: @Jenn Comment this method and unit test [-Jenn]
        public ResponseDto<bool> StoreSsoToken(SsoToken ssoToken)
        {
            using (var authenticationContext = new AuthenticationContext())
            {
                try
                {
                    authenticationContext.SsoTokens.Add(ssoToken);
                    authenticationContext.SaveChanges();

                    return new ResponseDto<bool>()
                    {
                        Data = true
                    };
                }
                catch (Exception)
                {
                    return new ResponseDto<bool>()
                    {
                        Data = false,
                        Error = "Something went wrong. Please try again later."
                    };
                }
            }
        }

        // Dispose release unmanaged resources 
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
