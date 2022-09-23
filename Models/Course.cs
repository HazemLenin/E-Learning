namespace E_Learning.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string TeacherId { get; set; }
        public virtual ApplicationUser? Teacher { get; set; }
        public virtual IEnumerable<Enrollment>? Enrollments { get; set; }
    }
}
