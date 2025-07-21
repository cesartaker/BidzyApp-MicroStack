using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserActivityHistoryConfiguration: IEntityTypeConfiguration<UserActivityHistory>
{
    public void Configure(EntityTypeBuilder<UserActivityHistory> builder)
    {
        builder.ToTable("UserActivityHistory");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .IsRequired();

        builder.Property(u => u.UserId)
               .IsRequired();

        builder.Property(u => u.Description)
               .HasMaxLength(500);

        builder.Property(u => u.Date)
               .IsRequired();

        // Opcional: Index para optimizar búsquedas por usuario
        builder.HasIndex(u => u.UserId);

    }
}
