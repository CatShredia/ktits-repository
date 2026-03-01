using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApi3K.Database.Models
{
    public class Users
    {
        [Key]
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [ForeignKey("Roles")]
        public int? id_Role { get; set; }
        public Roles Roles { get; set; }
    }
}
