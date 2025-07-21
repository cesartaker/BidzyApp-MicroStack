using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using System.Reflection.Emit;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id); // Clave primaria

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.MiddleName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.SecondLastName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.ToString(),
                Value => new Email(Value)
                )
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(u => u.PhoneNumber)
            .HasConversion(
                phoneNumber => phoneNumber.ToString(),
                Value => new PhoneNumber(Value)
                )
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(u => u.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.PhoneNumber);

       
    }
}
