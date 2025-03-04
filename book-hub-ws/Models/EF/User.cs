﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace book_hub_ws.Models.EF
{
    public class User
    {
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string? Community { get; set; }

        public ICollection<Book> Books { get; set; }

        [JsonIgnore]

        public ICollection<Conversation> InitiatedConversations { get; set; }

        [JsonIgnore]

        public ICollection<Conversation> ReceivedConversations { get; set; }

        public ICollection<UserGenre> UserGenres { get; set; }
    }


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

    public class UserGenre
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
