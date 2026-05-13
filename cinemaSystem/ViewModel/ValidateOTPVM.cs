using System.ComponentModel.DataAnnotations;

namespace cinemaSystem.ViewModel
{
    public class ValidateOTPVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "OTP")]
        public string OTP { get; set; } = string.Empty;
    }
}