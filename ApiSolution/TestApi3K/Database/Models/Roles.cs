using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestApi3K.Database.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Roles
    {
        [Key]
        public int id_Role { get; set; }
        public string Name { get; set; }
    }
}
