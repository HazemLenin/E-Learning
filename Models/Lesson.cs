namespace E_Learning.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Video { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
    }
}
