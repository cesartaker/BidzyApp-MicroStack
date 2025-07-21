using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.ID); // 🔹 Clave primaria

        builder.Property(r => r.Name)
            .IsRequired()
            .HasColumnType("VARCHAR(15)")
            .HasMaxLength(15);

        builder.HasIndex(r => r.Name); // 🔹 Índice para búsquedas rápidas

    }
}
