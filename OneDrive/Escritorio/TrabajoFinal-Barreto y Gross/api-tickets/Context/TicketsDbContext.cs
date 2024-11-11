using Microsoft.EntityFrameworkCore;

public class TicketsDbContext : DbContext
{
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<Estado> Estados {get; set;}

    public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Descripcion).IsRequired().HasMaxLength(1000);
        });

    }



}