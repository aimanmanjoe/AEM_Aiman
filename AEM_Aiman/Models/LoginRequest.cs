using System.ComponentModel.DataAnnotations;

namespace AEM_Aiman.Models
{
    public class LoginRequest
    {
        [Key]
        [MinLength(5)]
        public string UserName { get; set; }
        [MinLength(5)]
        public string Password { get; set; }
    }
}
