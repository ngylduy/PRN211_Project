using System.ComponentModel.DataAnnotations;

namespace dStore.Models
{
    public class Password
    {
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string NewPassword { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string ConfirmPassword { get; set; }
    }
}
