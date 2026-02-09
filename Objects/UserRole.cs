namespace fitness_dash_api.Objects
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public UserRole(string name) => Name = name;
    }
}
