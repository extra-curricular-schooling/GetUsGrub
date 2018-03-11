﻿using System.ComponentModel.DataAnnotations;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>IndividualProfileDto</c> class.
    /// Defines properties pertaining to a data transfer object of an individual profile.
    /// <para>
    /// @author: Jennifer Nguyen, Andrew Kao
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    public class UserProfileDto
    {
        public string DisplayPictureUrl { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
