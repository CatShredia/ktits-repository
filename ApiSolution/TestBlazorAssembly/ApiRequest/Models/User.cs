namespace TestBlazorAssembly.ApiRequest.Models
{
    public class UserDataShort
    {
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserData
    {
       public bool status { get; set; }
       public UserDataContainer data { get; set; }
    }

    public class UserDataContainer
    {
        public List<UserDataShort> users { get; set; }
    }

    public class ReqDataUser
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int id_Role { get; set; } = 2;
    }

    public class EditDataUser
    {
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int id_Role { get; set; } = 2;
    }

    public class UserAddData
    {
        public bool status { get; set; }
    }

    public class UserOperationResponse
    {
        public bool success { get; set; }
        public string? message { get; set; }
    }
}
