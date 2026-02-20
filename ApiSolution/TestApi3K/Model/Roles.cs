using System.ComponentModel.DataAnnotations;

namespace TestApi3K.Model;

public class Roles
{
    [Key]
    public int id_Role { get; set; }
    public string Name { get; set; }
}