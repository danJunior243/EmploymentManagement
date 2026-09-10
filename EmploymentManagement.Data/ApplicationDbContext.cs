using EmploymentManagement.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data
{
    public  class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        // Le contructeur qui recoit la configuration de la base de données via program.cs, et le transmet à la classe de base.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        // Ici nous allons creer les DBSet qui representent nos tables
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<Conges> Conges { get; set; }
        public DbSet<Absences> Absences { get; set; }
        public DbSet<Paie> Paies { get; set; }

       





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employe>().Property(e => e.Salaire).HasPrecision(10, 2);
            modelBuilder.Entity<Paie>().Property(p => p.SalaireBrut).HasPrecision(10, 2);
            modelBuilder.Entity<Paie>().Property(p => p.Deductions).HasPrecision(10, 2);
            modelBuilder.Entity<Paie>().Property(p => p.NetAPayer).HasPrecision(10, 2);
            modelBuilder.Entity<Absences>().Property(a => a.Duree).HasPrecision(10, 2);





            base.OnModelCreating(modelBuilder);

        }
    }
}
