﻿using System.ComponentModel.DataAnnotations;

namespace GitGrub.GetUsGrub.Models.Models
{
    class UserAccount
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required username.")]
        public string UserName { get; set; }

        // Stored as a hash
        [Required(ErrorMessage = "Required password.")]
        public string Password { get; set; }

        public string AccountType { get; set; }

        public bool IsActive { get; set; }
    }
}
