using Microsoft.EntityFrameworkCore;

namespace CExam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        
        // this is the variable we will use to connect to the MySQL table, User
        public DbSet<User> Users { get; set; }
        public DbSet<MeetUp> Activities { get; set; }
        public DbSet<Association> Associations { get; set; }

    }
}