namespace fitness_dash_api.Objects
{
    public class Exercise : BaseObject
    {
        
    }

    public class ExerciseCreate
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class ExerciseUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
