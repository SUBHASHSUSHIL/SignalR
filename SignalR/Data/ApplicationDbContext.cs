using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ChatRoom> ChatRoom { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
    }
}
