using System.ComponentModel.DataAnnotations;

namespace ProjectToDo.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
