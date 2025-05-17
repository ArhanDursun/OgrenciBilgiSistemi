using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OgrenciBilgiSistemi.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        [MaxLength(100)]
        public string Title { get; set; }

        // Bu alan formdan geliyor, zorunlu
        [Required(ErrorMessage = "Öğretmen seçmek zorunludur.")]
        public int TeacherId { get; set; }

        // Nav property’ler artık binding ve validation’dan muaf
        [ValidateNever]
        [BindNever]
        public User Teacher { get; set; }

        [ValidateNever]
        [BindNever]
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
