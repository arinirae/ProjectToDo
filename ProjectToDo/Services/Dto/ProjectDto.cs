namespace ProjectToDo.Services.Dto
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserPMId { get; set; }
        public string? UserPM { get; set; }
    }
}
