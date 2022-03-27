using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
