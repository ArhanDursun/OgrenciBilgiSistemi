using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OgrenciBilgiSistemi.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        // Bu derse atanan öğretmen
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public User Teacher { get; set; }

        // O derse kayıtlı öğrenciler
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
