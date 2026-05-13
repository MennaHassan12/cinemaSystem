using System.ComponentModel.DataAnnotations;

namespace cinemaSystem.ViewModel
{
    public class ResendEmailConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "EmailORUserName")]
        public string EmailORUserName { get; set; } = string.Empty;
    }
}
