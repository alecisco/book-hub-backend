using System.ComponentModel.DataAnnotations;


namespace book_hub_ws.Models.EF
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public int PublicationYear { get; set; }

        public string ISBN { get; set; }  
        public string Description { get; set; }  
        public string Condition { get; set; } 
        public string PhotoUrl { get; set; } 

        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        public int UserId { get; set; }

          public User? User { get; set; }
    }

    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string PhotoUrl { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string Status { get; set; } 
        public int UserId { get; set; }
    }

    public class BookCreateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public int PublicationYear { get; set; }

        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public bool Available { get; set; }
        public string PhotoUrl { get; set; }

        [Required]
        public int GenreId { get; set; }
    }

}
