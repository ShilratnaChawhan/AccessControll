using Microsoft.EntityFrameworkCore;

namespace AccessControll_API.ModelAndContext
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
        }
        public DbSet<Entity_User> Entity_User { get; set; }
        public DbSet<Entity_Person> Entity_Person { get; set; }
        public DbSet<Entity_Role> Entity_Role { get; set; }
        public DbSet<Entity_Menu> Entity_Menu { get; set; }
        public DbSet<Entity_Role_Menu_Permission> Entity_Role_Menu_Permission { get; set; }
        public DbSet<Entity_OTP_Log> Entity_OTP_Log { get; set; }

    }
}
