﻿using System.ComponentModel.DataAnnotations;

namespace CSULB.GetUsGrub.Models
{
    /// <summary>
    /// The <c>Address</c> class.
    /// Defines properties pertaining to an address.
    /// <para>
    /// @author: Brian Fann
    /// @updated: 03/10/2018
    /// </para>
    /// </summary>
    //[System.Serializable]
    public class Address : IAddress
    {
        // Automatic properties
        [Required]
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public int Zip { get; set; }

        // Constructors
        public Address() { }

        public Address(string city, string state)
        {
            City = city;
            State = state;
        }

        public Address(string street1, string street2, string city, string state, int zip)
        {
            Street1 = street1;
            Street2 = street2;
            City = city;
            State = state;
            Zip = zip;
        }

        public Address(string street1, string city, string state, int zip)
        {
            Street1 = street1;
            City = city;
            State = state;
            Zip = zip;
        }
    }
}
