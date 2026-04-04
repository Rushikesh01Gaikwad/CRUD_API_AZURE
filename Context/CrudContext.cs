using CRUD_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_API.Context
{
    public class CrudContext : DbContext
    {
        public CrudContext(DbContextOptions<CrudContext> options) : base(options)
        {
        }
        public DbSet<TeacherClass> Teachers { get; set; }
        public DbSet<DepartmentClass> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherClass>().HasOne(t => t.Department).WithMany(d => d.Teachers).HasForeignKey( t => t.DepartmentID);
            modelBuilder.Entity<DepartmentClass>().HasMany(d => d.Teachers).WithOne(t => t.Department).HasForeignKey(t => t.DepartmentID);
        }
    }
}
