using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OgrenciBilgiSistemi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Boş Bırakılamaz Ve Minimum uzunluk 6 Maksimum uzunluk 20 karakter olmalı")]
        [MinLength(6),MaxLength(20)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; }    // "Admin", "Teacher", "Student"

        // FOTOĞRAF BİLGİSİ
        [ValidateNever]
        [BindNever]
        [Column(TypeName = "varbinary(max)")]
        public byte[] Photo { get; set; }

        // Yalnızca upload için, veritabanına yansımıyor
        [NotMapped]
        [Required(ErrorMessage = "Profil fotoğrafı yüklemek zorunludur.")]
        public IFormFile PhotoFile { get; set; }

        // Navigation
        [ValidateNever]
        [BindNever]
        public ICollection<Course> CoursesTeaching { get; set; }
        
        [ValidateNever]
        [BindNever]
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
