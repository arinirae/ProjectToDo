﻿namespace ProjectToDo.Services.Dto
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int StatusId { get; set; }
        public string? UserName { get; set; }
        public string? ProjectName { get; set; }
        public string? Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}