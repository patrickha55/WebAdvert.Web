using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class SignInModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 6)]
        public string Password { get; set; }
    }
}
