﻿using CSULB.GetUsGrub.DataAccess;
using CSULB.GetUsGrub.Models;
using System.Configuration;
using System.IO;
using System.Web;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// Performs business logic and executes requests regarding restaurant profiles
    /// 
    /// @author: Andrew Kao
    /// @updated: 3/18/18
    /// </summary>
    public class RestaurantProfileManager : IProfileManager<RestaurantProfileDto>
    {
        public ResponseDto<RestaurantProfileDto> GetProfile(string token)
        {
            // Retrieve account by username
            var userGateway = new UserGateway();

            var tokenService = new TokenService();

            var userAccountResponseDto = userGateway.GetUserByUsername(tokenService.GetTokenUsername(token));

            // Retrieve restaurant profile from database
            var restaurantProfileGateway = new RestaurantProfileGateway();

            var restaurantProfileResponseDto = restaurantProfileGateway.GetRestaurantProfileById(userAccountResponseDto.Data.Id);

            return restaurantProfileResponseDto;
        }

        public ResponseDto<bool> EditProfile(RestaurantProfileDto restaurantProfileDto, string token)
        {
            var editRestaurantProfilePreLogicValidationStrategy = new EditRestaurantUserProfilePreLogicValidationStrategy(restaurantProfileDto);

            var result = editRestaurantProfilePreLogicValidationStrategy.ExecuteStrategy();

            if (result.Error != null)
            {
                return new ResponseDto<bool>
                {
                    Data = false,
                    Error = "Something went wrong. Please try again later."
                };
            }

            // Extract DTO contents and map DTO to domain model
            var tokenService = new TokenService();

            // Extract restaurant profile domain
            var restaurantProfileDomain = new RestaurantProfile(
                restaurantProfileDto.PhoneNumber,
                restaurantProfileDto.Address,
                restaurantProfileDto.Details,
                restaurantProfileDto.GeoCoordinates.Latitude,
                restaurantProfileDto.GeoCoordinates.Longitude);

            // Extract business hours domains
            var businessHourDomains = restaurantProfileDto.BusinessHours;


            // Extract restaurant menu dictionary
            var restaurantMenuDomains = restaurantProfileDto.RestaurantMenusList;

            // Extract menu items

            // Execute update of database
            var profileGateway = new RestaurantProfileGateway();

            var responseDtoFromGateway = profileGateway.EditRestaurantProfile(tokenService.GetTokenUsername(token), restaurantProfileDomain, businessHourDomains, restaurantMenuDomains);

            return responseDtoFromGateway;
        }

        //ImageUploadManager
        // TODO: @Angelica Add image profile upload here
        public ResponseDto<bool> MenuItemImageUpload(HttpPostedFile image, string username, string menuItem, string ItemName)
        {
            var user = new UserProfileDto() { Username = username };

            var ImageUploadValidationStrategy = new ImageUploadValidationStrategy(user, image);
            var result = ImageUploadValidationStrategy.ExecuteStrategy();

            if (result.Data == false)
            {
                return new ResponseDto<bool>()
                {
                    Data = false,
                    Error = result.Error
                };
            }

            var renameImage = Path.GetExtension(image.FileName);
            var newImagename = username + "_" + menuItem + "_" + ItemName  + renameImage;

            // Save image to path
            string savePath = ConfigurationManager.AppSettings["MenuImagePath"];
            //string fileName = Path.GetFileName(image.FileName);// file name should be username.png
            //Debug.WriteLine(fileName);
            var menuPath = savePath +  newImagename; // Store image path to DTO

            // Call gateway to save path to database
            using (var gateway = new RestaurantProfileGateway())
            {
                var gatewayresult = gateway.UploadImage(user, menuPath, menuItem, ItemName);
                if (gatewayresult.Data == false)
                {
                    return new ResponseDto<bool>()
                    {
                        Data = false,
                        Error = gatewayresult.Error
                    };
                }

                image.SaveAs(menuPath);

                return new ResponseDto<bool>
                {
                    Data = true
                };
            }
        }
    }
}