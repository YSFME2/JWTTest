using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTTest.Models
{
    public class UserRole
    {
        [Required]
        public string UserID { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
