using Microsoft.EntityFrameworkCore;
using EsepteSite.Models;
using System;

namespace EsepteSite.Data {

    // контекст базы данных для приложения
    public class ApplicationDbContext : DbContext {
        
        public DbSet<Messages> Messages { get; set; } 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}