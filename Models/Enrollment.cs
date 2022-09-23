namespace E_Learning.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public double Cost { get; set; }
        public string StudentId { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
    }
}
