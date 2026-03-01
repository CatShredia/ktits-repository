namespace TestApi3K.Requests
{
    public class CreateNewUserAndLogin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int id_Role { get; set; }
    }

    public class EditUserAndLogin
    {
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int id_Role { get; set; }
    }
}
