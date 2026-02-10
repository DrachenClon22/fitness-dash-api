namespace fitness_dash_api.Objects
{
    public class User : BaseObject
    {
        public string Name { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
    }

    public class LoginRequest
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
