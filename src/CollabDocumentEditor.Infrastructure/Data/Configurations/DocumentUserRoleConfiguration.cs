using CollabDocumentEditor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollabDocumentEditor.Infrastructure.Data.Configurations;

public class DocumentUserRoleConfiguration : IEntityTypeConfiguration<DocumentUserPermission>
{
   public void Configure(EntityTypeBuilder<DocumentUserPermission> builder)
   {
      builder.HasKey(x => x.Id);

      // Index for each user can have only one active permission for a document
      builder.HasIndex(x => new { x.DocumentId, x.UserId })
         .IsUnique()
         .HasFilter("\"RevokedAt\" IS NULL");

      builder.Property(x => x.Role)
         .HasConversion<string>()
         .IsRequired();

      builder.HasOne(x => x.Document)
         .WithMany(d => d.DocumentUserPermissions)
         .HasForeignKey(x => x.DocumentId)
         .OnDelete(DeleteBehavior.Cascade);

      builder.HasOne(x => x.User)
         .WithMany()
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);
   }
}