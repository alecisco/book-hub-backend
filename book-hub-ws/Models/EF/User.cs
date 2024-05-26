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
    }

}
