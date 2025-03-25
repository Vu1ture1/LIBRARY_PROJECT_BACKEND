using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BooksApi.Entities;

namespace BooksApi.Entities.EntitiesConfiguration
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Surname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Counry)
                .IsRequired()
                .HasMaxLength(100);
        }
    }

}
