using AEM_Aiman.Models;
using Microsoft.EntityFrameworkCore;



namespace AEM_Aiman.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Well> Wells { get; set; }

        public DbSet<LoginRequest> LoginRequests { get; set; }
       
   
    }
}
