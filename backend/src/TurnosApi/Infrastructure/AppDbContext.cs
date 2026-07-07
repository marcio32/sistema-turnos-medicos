using Microsoft.EntityFrameworkCore;
using TurnosApi.Models;

namespace TurnosApi.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Especialidad> Especialidades => Set<Especialidad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================================
        // Especialidad
        // =====================================================================
        modelBuilder.Entity<Especialidad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
        });

        // =====================================================================
        // Medico
        // =====================================================================
        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Matricula).IsRequired().HasMaxLength(50);

            // Índice único en Matrícula
            entity.HasIndex(e => e.Matricula).IsUnique();

            // Relación 1:N — Especialidad → Médicos
            entity.HasOne(e => e.Especialidad)
                  .WithMany(esp => esp.Medicos)
                  .HasForeignKey(e => e.EspecialidadId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================================
        // Paciente
        // =====================================================================
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(50);
            entity.Property(e => e.DNI).IsRequired().HasMaxLength(20);

            // Índice único en DNI
            entity.HasIndex(e => e.DNI).IsUnique();
        });

        // =====================================================================
        // Turno
        // =====================================================================
        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Motivo).HasMaxLength(500);
            entity.Property(e => e.Estado)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // Índice compuesto: MedicoId + Fecha (consultas de disponibilidad)
            entity.HasIndex(e => new { e.MedicoId, e.Fecha });

            // Índice: PacienteId (consultas de turnos por paciente)
            entity.HasIndex(e => e.PacienteId);

            // Relación 1:N — Medico → Turnos
            entity.HasOne(e => e.Medico)
                  .WithMany(m => m.Turnos)
                  .HasForeignKey(e => e.MedicoId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación 1:N — Paciente → Turnos
            entity.HasOne(e => e.Paciente)
                  .WithMany(p => p.Turnos)
                  .HasForeignKey(e => e.PacienteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================================
        // Seed Data
        // =====================================================================
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // --- Especialidades ---
        modelBuilder.Entity<Especialidad>().HasData(
            new Especialidad { Id = 1, Nombre = "Clínica Médica", Descripcion = "Medicina general y atención primaria" },
            new Especialidad { Id = 2, Nombre = "Cardiología", Descripcion = "Diagnóstico y tratamiento de enfermedades cardiovasculares" },
            new Especialidad { Id = 3, Nombre = "Pediatría", Descripcion = "Atención médica de niños y adolescentes" }
        );

        // --- Médicos ---
        modelBuilder.Entity<Medico>().HasData(
            new Medico { Id = 1, Nombre = "Dra. Ana García", Matricula = "MN-12345", EspecialidadId = 1 },
            new Medico { Id = 2, Nombre = "Dr. Carlos Rodríguez", Matricula = "MN-23456", EspecialidadId = 2 },
            new Medico { Id = 3, Nombre = "Dra. Laura Martínez", Matricula = "MN-34567", EspecialidadId = 3 }
        );

        // --- Pacientes ---
        modelBuilder.Entity<Paciente>().HasData(
            new Paciente { Id = 1, Nombre = "Juan Pérez", Email = "juan.perez@email.com", Telefono = "11-2345-6789", DNI = "30123456" },
            new Paciente { Id = 2, Nombre = "María López", Email = "maria.lopez@email.com", Telefono = "11-3456-7890", DNI = "31234567" },
            new Paciente { Id = 3, Nombre = "Roberto Fernández", Email = "roberto.fernandez@email.com", Telefono = "11-4567-8901", DNI = "32345678" }
        );
    }
}
