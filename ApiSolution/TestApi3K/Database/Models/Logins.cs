using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestApi3K.Database.Models
{
    [Index(nameof(Login), IsUnique = true)]
    public class Logins
    {
        [Key]
        public int id_Login { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int User_id { get; set; }
        public Users Users { get; set; }
    }
}
