using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksApi.Entities.EntitiesConfiguration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(b => b.Genre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .HasMaxLength(1000);

            builder.Property(b => b.Image)
                .HasMaxLength(255);

            builder.HasOne(b => b.book_author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
