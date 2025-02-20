using Microsoft.EntityFrameworkCore;


namespace WebAPITemplate.DataDBContext
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
            
        }
        //APIKeysModel
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
          
        }
    }
}
