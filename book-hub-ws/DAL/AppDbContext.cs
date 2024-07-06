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
        }
    }
}
