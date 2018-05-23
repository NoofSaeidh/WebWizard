using System.ComponentModel.DataAnnotations;

namespace PX.WebWizard.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}