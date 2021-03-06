﻿using CSULB.GetUsGrub.Models;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// The abstract <c>RestaurantSelectionManager</c> class.
    /// Created an abstract base class to reduce code redundancy.
    /// Contains all methods and properties in regards to selecting a restaurant for a user.
    /// <para>
    /// @author: Jennifer Nguyen
    /// @updated: 04/08/2018
    /// </para>
    /// </summary>
    public abstract class RestaurantSelectionManager
    {
        // Constants
        public const decimal MilesToMetersUnitConversion = 1609.344M;

        // Read-only accessors
        public readonly RestaurantSelectionDto RestaurantSelectionDto;

        // Read and write accessors
        public SelectedRestaurantDto SelectedRestaurantDto;

        // Protected read-only accessors
        protected readonly RestaurantSelectionPreLogicValidationStrategy _restaurantSelectionPreLogicValidationStrategy;
        protected readonly GoogleGeocodeService _geocodeService;
        protected readonly GoogleTimeZoneService _timeZoneService;
        protected readonly DateTimeService _dateTimeService;

        protected RestaurantSelectionManager(RestaurantSelectionDto restaurantSelectionDto)
        {
            RestaurantSelectionDto = restaurantSelectionDto;
            SelectedRestaurantDto = new SelectedRestaurantDto();
            _restaurantSelectionPreLogicValidationStrategy = new RestaurantSelectionPreLogicValidationStrategy(restaurantSelectionDto);
            _geocodeService = new GoogleGeocodeService();
            _timeZoneService = new GoogleTimeZoneService();
            _dateTimeService = new DateTimeService();
        }

        public abstract ResponseDto<SelectedRestaurantDto> SelectRestaurant();
        
        /// <summary>
        /// The ConvertDistanceInMilesToMeters method.
        /// Converts a distance in miles to meters.
        /// <para>
        /// @author: Jennifer Nguyen
        /// @update: 04/04/2018
        /// </para>
        /// </summary>
        /// <param name="distanceInMiles"></param>
        /// <returns>A double type that represents the distance in meters.</returns>
        public decimal ConvertDistanceInMilesToMeters(decimal distanceInMiles)
        {
            return distanceInMiles * MilesToMetersUnitConversion;
        }
    }
}