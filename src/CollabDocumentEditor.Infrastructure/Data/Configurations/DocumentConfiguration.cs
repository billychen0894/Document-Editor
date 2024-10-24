using CollabDocumentEditor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabDocumentEditor.Infrastructure.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
   public void Configure(EntityTypeBuilder<Document> builder)
   {
       builder.HasKey(d => d.Id);

       builder.Property(d => d.Title)
           .IsRequired()
           .HasMaxLength(200);

       builder.Property(d => d.Content)
           .IsRequired();
       
       builder.Property(d => d.CreatedAt)
           .IsRequired();
       
       builder.Property(d => d.UpdatedAt)
           .IsRequired();
       
       builder.Property(d => d.UserId)
           .IsRequired();
       
       builder.HasIndex(d => d.UserId);
       builder.HasIndex(d => d.CreatedAt);

       // One-to-Many relationship: one user can have many documents
       builder.HasOne(d => d.User)
           .WithMany(u => u.Documents)
           .HasForeignKey(d => d.UserId)
           .OnDelete(DeleteBehavior.Cascade);
   }
}