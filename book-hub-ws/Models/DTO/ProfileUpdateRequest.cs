using System.ComponentModel.DataAnnotations;

public class ProfileUpdateRequest
{
    [Required]
    public string Nickname { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    public string Community { get; set; }
}
