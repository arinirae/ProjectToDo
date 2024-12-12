using System.ComponentModel.DataAnnotations;

namespace ProjectToDo.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        // FK
        public int UserPMId { get; set; }
        public User User { get; set; }

        public ICollection<Task> Tasks { get; set; }
    }
}
