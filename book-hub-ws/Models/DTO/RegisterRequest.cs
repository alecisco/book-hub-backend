using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    public string Nickname { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    [Required]
    public string Password { get; set; }

    public string? Community { get; set; }
}
