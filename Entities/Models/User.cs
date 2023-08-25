using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("IDUser")]
        public int Id { get; set; }

        [ForeignKey(nameof(Person))]
        [Required(ErrorMessage = "IDPerson is required")]
        public Guid IDPerson { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Salt is required")]
        public string Salt { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; }

        [Required(ErrorMessage = "RefreshTokenExpireDate is required")]
        public DateTime? RefreshTokenExpireDate { get; set; }


        // Add roles and permissions properties
        [NotMapped]
        public ICollection<string> Roles
        {
            get
            {
                return new List<string> { Role };
            }
        }

        [NotMapped]
        public ICollection<string> Permissions
        {
            get
            {
                // Define permissions based on the user's role
                if (Role == "ADMIN")
                {
                    return new List<string> { "CanEdit", "CanDelete" };
                }
                else if (Role == "USER")
                {
                    return new List<string> { "CanView" };
                }
                else
                {
                    return new List<string>();
                }
            }
        }
    }
}
