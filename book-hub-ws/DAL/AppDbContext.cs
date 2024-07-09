using book_hub_ws.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace book_hub_ws.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanRequest> LoanRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<UserGenre> UserGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany()
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<LoanRequest>()
                .HasOne(lr => lr.Book)
                .WithMany()
                .HasForeignKey(lr => lr.BookId);

            modelBuilder.Entity<LoanRequest>()
                .HasOne(lr => lr.RequesterUser)
                .WithMany()
                .HasForeignKey(lr => lr.RequesterUserId);

            modelBuilder.Entity<LoanRequest>()
                .HasOne(lr => lr.SpecificBook)
                .WithMany()
                .HasForeignKey(lr => lr.SpecificBookId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.LoanRequest)
                .WithMany()
                .HasForeignKey(r => r.LoanRequestId);

            modelBuilder.Entity<Loan>()
          .HasOne(l => l.Book)
          .WithMany()
          .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.InitiatedConversations)
                .WithOne(c => c.InitiatorUser)
                .HasForeignKey(c => c.InitiatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedConversations)
                .WithOne(c => c.ReceiverUser)
                .HasForeignKey(c => c.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserGenre>()
            .HasKey(ug => new { ug.UserId, ug.GenreId });

            modelBuilder.Entity<UserGenre>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGenres)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGenre>()
                .HasOne(ug => ug.Genre)
                .WithMany(g => g.UserGenres)
                .HasForeignKey(ug => ug.GenreId);
        }
    }
}
