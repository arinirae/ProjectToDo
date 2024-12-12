using System.ComponentModel.DataAnnotations;

namespace ProjectToDo.Models
{
    public class Task
    {
        public int Id { get; set; }

        //FK
        public int UserId { get; set; }
        public User User { get; set; }

        //FK
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        //FK
        public int StatusId { get; set; }
        public Status Status { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime ExpiredDate { get; set; }


    }
}
