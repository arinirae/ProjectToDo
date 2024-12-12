using System.ComponentModel.DataAnnotations;

namespace ProjectToDo.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        // FK
        public int RoleId { get; set; }
        public Role Role { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Task> Tasks { get; set; }

    }
}
