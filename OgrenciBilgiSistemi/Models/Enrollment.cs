using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciBilgiSistemi.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        // Hangi öğrenci
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public User Student { get; set; }

        // Hangi ders
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
